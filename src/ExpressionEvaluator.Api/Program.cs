using System.Threading.RateLimiting;
using ExpressionEvaluator.Api.Services;
using ExpressionEvaluator.Core;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddKeyedSingleton<IExpressionEvaluator, BruteForceExpressionEvaluator>("bruteForce");
builder.Services.AddKeyedSingleton<IExpressionEvaluator, OptimizedExpressionEvaluator>("optimized");
builder.Services.AddSingleton<IExpressionEvaluationService, ExpressionEvaluationService>();

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
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }
