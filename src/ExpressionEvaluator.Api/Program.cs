using System.Security.Cryptography;
using System.Text;
using System.Threading.RateLimiting;
using ExpressionEvaluator.Api.Data;
using ExpressionEvaluator.Api.Security;
using ExpressionEvaluator.Api.Services;
using ExpressionEvaluator.Core;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Render (and similar PaaS hosts) assign a port at runtime via PORT and route
// traffic to it; honor it when present instead of the fixed dev port.
var renderPort = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrEmpty(renderPort))
{
    builder.WebHost.UseUrls($"http://0.0.0.0:{renderPort}");
}

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddKeyedSingleton<IExpressionEvaluator, BruteForceExpressionEvaluator>("bruteForce");
builder.Services.AddKeyedSingleton<IExpressionEvaluator, OptimizedExpressionEvaluator>("optimized");
builder.Services.AddSingleton<IExpressionEvaluationService, ExpressionEvaluationService>();

var connectionString = builder.Configuration.GetConnectionString("Default") ?? "Data Source=expressionevaluator.db";
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(connectionString));
builder.Services.AddScoped<IAuthService, AuthService>();

// JWT signing key: read from configuration (environment variable / user-secrets)
// so it is never committed. In Development only, fall back to a key generated
// fresh for this process, since a local demo doesn't need stable keys across
// restarts (tokens just need to be re-issued via /api/auth/login after a restart).
var configuredKey = builder.Configuration["Jwt:Key"];
byte[] signingKeyBytes;
if (!string.IsNullOrEmpty(configuredKey))
{
    signingKeyBytes = Encoding.UTF8.GetBytes(configuredKey);
}
else if (builder.Environment.IsDevelopment())
{
    signingKeyBytes = RandomNumberGenerator.GetBytes(32);
}
else
{
    throw new InvalidOperationException("Jwt:Key must be configured (environment variable or secret store) outside Development.");
}

var jwtSettings = new JwtSettings
{
    Key = signingKeyBytes,
    Issuer = builder.Configuration["Jwt:Issuer"] ?? "ExpressionEvaluator.Api",
    Audience = builder.Configuration["Jwt:Audience"] ?? "ExpressionEvaluator.Dashboard",
    ExpiryMinutes = builder.Configuration.GetValue("Jwt:ExpiryMinutes", 60)
};
builder.Services.AddSingleton(jwtSettings);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(jwtSettings.Key)
        };
    });
builder.Services.AddAuthorization();

// Basic abuse protection: cap requests per client so the evaluation endpoint
// can't be used to hammer the service (defense in depth alongside the input
// length cap enforced in ExpressionEvaluator.Core.Tokenizer).
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "anonymous",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 60,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            }));
});

var originMatcher = new OriginMatcher(
    builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? [],
    builder.Configuration.GetSection("AllowedOriginPatterns").Get<string[]>() ?? []);

builder.Services.AddCors(options =>
{
    options.AddPolicy("Dashboard", policy =>
        policy.SetIsOriginAllowed(originMatcher.IsAllowed)
              .AllowAnyHeader()
              .WithMethods("GET", "POST"));
});

// Behind Render's proxy every request arrives from the proxy's IP, which would
// put all users in one rate-limit bucket. Trust only the last X-Forwarded-For
// hop (the one the proxy appends), so clients can't spoof their own IP.
if (!string.IsNullOrEmpty(renderPort))
{
    builder.Services.Configure<ForwardedHeadersOptions>(options =>
    {
        options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
        options.ForwardLimit = 1;
        options.KnownIPNetworks.Clear();
        options.KnownProxies.Clear();
    });
}

var app = builder.Build();

// Ensure the database exists and seed one admin user on first run.
// - Seed:AdminPassword set (env var Seed__AdminPassword): use it, never log it.
// - Not set, Development: generate one and log it once to the local console.
// - Not set, any hosted environment: skip seeding rather than print a password
//   into the host's log stream; users can still self-register.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();

    if (!db.Users.Any())
    {
        var logger = app.Services.GetRequiredService<ILogger<Program>>();
        var configuredPassword = app.Configuration["Seed:AdminPassword"];
        string? adminPassword = null;

        if (!string.IsNullOrEmpty(configuredPassword))
        {
            adminPassword = configuredPassword;
            logger.LogInformation("Seeded admin user from Seed:AdminPassword configuration.");
        }
        else if (app.Environment.IsDevelopment())
        {
            adminPassword = Convert.ToBase64String(RandomNumberGenerator.GetBytes(12))
                .Replace("+", "A").Replace("/", "B").Replace("=", "C");
            logger.LogWarning(
                "Seeded default admin user on first run. Username: {Username}  Password: {Password}  " +
                "Shown once, local Development only - save it now.",
                "admin", adminPassword);
        }
        else
        {
            logger.LogInformation("No Seed:AdminPassword configured; skipping admin seed. Users can self-register.");
        }

        if (adminPassword is not null)
        {
            db.Users.Add(new User
            {
                Username = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(adminPassword),
                Role = "Admin"
            });
            db.SaveChanges();
        }
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.Use(async (context, next) =>
{
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Append("X-Frame-Options", "DENY");
    context.Response.Headers.Append("Referrer-Policy", "no-referrer");
    await next();
});

if (!string.IsNullOrEmpty(renderPort))
{
    app.UseForwardedHeaders();
}

// CORS runs before the rate limiter so a 429 still carries CORS headers -
// otherwise the browser reports it as a misleading CORS error.
app.UseCors("Dashboard");
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/health", () => Results.Ok(new { status = "ok" }))
   .AllowAnonymous()
   .DisableRateLimiting();

app.MapControllers();

app.Run();

public partial class Program { }
