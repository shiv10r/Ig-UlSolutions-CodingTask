namespace ExpressionEvaluator.Api.Services;

public interface IAuthService
{
    /// <summary>Returns a signed JWT if the credentials are valid, otherwise null.</summary>
    Task<string?> LoginAsync(string username, string password);
}
