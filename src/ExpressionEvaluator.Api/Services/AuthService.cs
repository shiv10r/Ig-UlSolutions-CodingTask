using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ExpressionEvaluator.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ExpressionEvaluator.Api.Services;

public class AuthService(AppDbContext db, JwtSettings jwtSettings) : IAuthService
{
    public async Task<string?> LoginAsync(string username, string password)
    {
        var user = await db.Users.SingleOrDefaultAsync(u => u.Username == username);
        if (user is null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            return null;

        return GenerateToken(user);
    }

    public async Task<(RegisterOutcome Outcome, string? Token)> RegisterAsync(string username, string password)
    {
        var exists = await db.Users.AnyAsync(u => u.Username == username);
        if (exists) return (RegisterOutcome.UsernameTaken, null);

        var user = new User
        {
            Username = username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            Role = "User"
        };
        db.Users.Add(user);
        await db.SaveChangesAsync();

        return (RegisterOutcome.Created, GenerateToken(user));
    }

    private string GenerateToken(User user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Username),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var credentials = new SigningCredentials(new SymmetricSecurityKey(jwtSettings.Key), SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: jwtSettings.Issuer,
            audience: jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(jwtSettings.ExpiryMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
