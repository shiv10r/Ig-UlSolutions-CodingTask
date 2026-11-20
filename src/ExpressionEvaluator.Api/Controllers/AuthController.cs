using ExpressionEvaluator.Api.Models;
using ExpressionEvaluator.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpressionEvaluator.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        var token = await authService.LoginAsync(request.Username, request.Password);
        if (token is null)
            return Unauthorized(new { message = "Invalid username or password." });

        return Ok(new LoginResponse { Token = token, Username = request.Username, ExpiresInMinutes = 60 });
    }

    /// <summary>
    /// JWTs are stateless, so there is no server-side session to end: this
    /// endpoint just confirms the caller still holds a valid token. The
    /// client is responsible for discarding it to actually "log out".
    /// </summary>
    [HttpPost("logout")]
    [Authorize]
    public IActionResult Logout() => Ok(new { message = "Logged out. Discard the token client-side." });
}
