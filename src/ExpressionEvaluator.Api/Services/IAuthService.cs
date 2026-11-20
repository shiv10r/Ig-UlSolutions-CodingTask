namespace ExpressionEvaluator.Api.Services;

public enum RegisterOutcome { Created, UsernameTaken }

public interface IAuthService
{
    /// <summary>Returns a signed JWT if the credentials are valid, otherwise null.</summary>
    Task<string?> LoginAsync(string username, string password);

    /// <summary>Creates a new user (role "User") and, on success, a JWT for immediate sign-in.</summary>
    Task<(RegisterOutcome Outcome, string? Token)> RegisterAsync(string username, string password);
}
