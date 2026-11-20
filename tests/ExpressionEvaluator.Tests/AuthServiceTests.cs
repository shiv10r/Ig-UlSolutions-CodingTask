using System.Text;
using ExpressionEvaluator.Api.Data;
using ExpressionEvaluator.Api.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ExpressionEvaluator.Tests;

public class AuthServiceTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly AppDbContext _db;
    private readonly AuthService _sut;

    public AuthServiceTests()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>().UseSqlite(_connection).Options;
        _db = new AppDbContext(options);
        _db.Database.EnsureCreated();

        _db.Users.Add(new User
        {
            Username = "admin",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Secret123!"),
            Role = "Admin"
        });
        _db.SaveChanges();

        var jwtSettings = new JwtSettings
        {
            Key = Encoding.UTF8.GetBytes("unit-test-signing-key-32-bytes-minimum"),
            Issuer = "test-issuer",
            Audience = "test-audience",
            ExpiryMinutes = 5
        };
        _sut = new AuthService(_db, jwtSettings);
    }

    [Fact]
    public async Task LoginAsync_CorrectCredentials_ReturnsJwt()
    {
        var token = await _sut.LoginAsync("admin", "Secret123!");

        Assert.NotNull(token);
        Assert.Equal(3, token!.Split('.').Length); // header.payload.signature
    }

    [Fact]
    public async Task LoginAsync_WrongPassword_ReturnsNull()
    {
        Assert.Null(await _sut.LoginAsync("admin", "wrong-password"));
    }

    [Fact]
    public async Task LoginAsync_UnknownUsername_ReturnsNull()
    {
        Assert.Null(await _sut.LoginAsync("nobody", "Secret123!"));
    }

    [Fact]
    public async Task RegisterAsync_NewUsername_CreatesUserAndReturnsToken()
    {
        var (outcome, token) = await _sut.RegisterAsync("newuser", "Password123!");

        Assert.Equal(RegisterOutcome.Created, outcome);
        Assert.NotNull(token);
        Assert.NotNull(await _sut.LoginAsync("newuser", "Password123!"));
    }

    [Fact]
    public async Task RegisterAsync_UsernameAlreadyTaken_ReturnsConflictOutcome()
    {
        var (outcome, token) = await _sut.RegisterAsync("admin", "AnotherPass1!");

        Assert.Equal(RegisterOutcome.UsernameTaken, outcome);
        Assert.Null(token);
    }

    public void Dispose()
    {
        _db.Dispose();
        _connection.Dispose();
    }
}
