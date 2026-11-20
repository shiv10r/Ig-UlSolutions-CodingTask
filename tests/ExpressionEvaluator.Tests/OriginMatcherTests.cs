using ExpressionEvaluator.Api.Security;
using Xunit;

namespace ExpressionEvaluator.Tests;

public class OriginMatcherTests
{
    private readonly OriginMatcher _sut = new(
        ["http://localhost:5173", "https://dashboard.example.com/"],
        ["https://dashboard-*-vsr11.vercel.app", "*"]);

    [Theory]
    [InlineData("http://localhost:5173")]
    [InlineData("https://dashboard.example.com")]
    [InlineData("https://DASHBOARD.example.com")]
    [InlineData("https://dashboard-11vw2yhzz-vsr11.vercel.app")]
    [InlineData("https://dashboard-abc123-vsr11.vercel.app")]
    public void IsAllowed_ConfiguredOrigins_ReturnsTrue(string origin)
    {
        Assert.True(_sut.IsAllowed(origin));
    }

    [Theory]
    [InlineData("")]
    [InlineData("http://localhost:3000")]
    [InlineData("http://dashboard-11vw2yhzz-vsr11.vercel.app")]
    [InlineData("https://dashboard-x.evil.com-vsr11.vercel.app")]
    [InlineData("https://dashboard-abc-vsr11.vercel.app.evil.com")]
    [InlineData("https://dashboard-abc-otherteam.vercel.app")]
    [InlineData("https://evil.com")]
    public void IsAllowed_UnlistedOrSpoofedOrigins_ReturnsFalse(string origin)
    {
        Assert.False(_sut.IsAllowed(origin));
    }
}
