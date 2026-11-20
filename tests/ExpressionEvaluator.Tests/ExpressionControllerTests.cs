using ExpressionEvaluator.Api.Controllers;
using ExpressionEvaluator.Api.Models;
using ExpressionEvaluator.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace ExpressionEvaluator.Tests;

public class ExpressionControllerTests
{
    private readonly ExpressionController _sut = new(
        NullLogger<ExpressionController>.Instance,
        new BruteForceExpressionEvaluator(),
        new OptimizedExpressionEvaluator());

    [Fact]
    public void Evaluate_ValidExpression_ReturnsOkWithResult()
    {
        var result = _sut.Evaluate(new EvaluateRequest { Expression = "4+5*2" });

        var ok = Assert.IsType<OkObjectResult>(result);
        var body = Assert.IsType<EvaluateResponse>(ok.Value);
        Assert.Equal(14, body.Result);
        Assert.Equal("optimized", body.Algorithm);
    }

    [Fact]
    public void EvaluateBruteForce_ValidExpression_ReturnsOkWithResult()
    {
        var result = _sut.EvaluateBruteForce(new EvaluateRequest { Expression = "4+5/2-1" });

        var ok = Assert.IsType<OkObjectResult>(result);
        var body = Assert.IsType<EvaluateResponse>(ok.Value);
        Assert.Equal(5.5, body.Result);
        Assert.Equal("brute-force", body.Algorithm);
    }

    [Fact]
    public void Compare_ValidExpression_BothAlgorithmsAgree()
    {
        var result = _sut.Compare(new EvaluateRequest { Expression = "4+5/2" });

        var ok = Assert.IsType<OkObjectResult>(result);
        var body = Assert.IsType<EvaluateCompareResponse>(ok.Value);
        Assert.True(body.Match);
        Assert.Equal(6.5, body.BruteForceResult);
        Assert.Equal(6.5, body.OptimizedResult);
    }

    [Fact]
    public void Evaluate_InvalidExpression_ReturnsBadRequest()
    {
        var result = _sut.Evaluate(new EvaluateRequest { Expression = "4+5/0" });

        var problem = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status400BadRequest, problem.StatusCode);
    }
}
