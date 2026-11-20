namespace ExpressionEvaluator.Api.Services;

/// <summary>
/// Resolved JWT signing configuration, built once in Program.cs and shared
/// between the token-issuing (AuthService) and token-validating
/// (JwtBearer middleware) sides so they always agree on the same key.
/// </summary>
public class JwtSettings
{
    public required byte[] Key { get; init; }
    public required string Issuer { get; init; }
    public required string Audience { get; init; }
    public int ExpiryMinutes { get; init; } = 60;
}
