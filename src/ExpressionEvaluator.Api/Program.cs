using System.Security.Cryptography;
using System.Text;
using System.Threading.RateLimiting;
using ExpressionEvaluator.Api.Data;
using ExpressionEvaluator.Api.Services;
using ExpressionEvaluator.Core;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddCors(options =>
{
    options.AddPolicy("Dashboard", policy =>
        policy.WithOrigins(builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? [])
              .AllowAnyHeader()
              .WithMethods("GET", "POST"));
});

var app = builder.Build();

// Ensure the local SQLite database exists and seed a single admin user on
// first run only. The generated password is logged once (Warning level) and
// is never written anywhere else — not to the database in plaintext, not to
// any file in this repo.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();

    if (!db.Users.Any())
    {
        var generatedPassword = Convert.ToBase64String(RandomNumberGenerator.GetBytes(12))
            .Replace("+", "A").Replace("/", "B").Replace("=", "C");

        db.Users.Add(new User
        {
            Username = "admin",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(generatedPassword),
            Role = "Admin"
        });
        db.SaveChanges();

        var logger = app.Services.GetRequiredService<ILogger<Program>>();
        logger.LogWarning(
            "Seeded default admin user on first run. Username: {Username}  Password: {Password}  " +
            "This password is shown here ONCE and is not stored anywhere else in this repo — save it now and change it after first login.",
            "admin", generatedPassword);
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

app.UseRateLimiter();
app.UseCors("Dashboard");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }
