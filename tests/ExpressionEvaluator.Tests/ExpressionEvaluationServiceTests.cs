using ExpressionEvaluator.Api.Services;
using ExpressionEvaluator.Core;
using Xunit;

namespace ExpressionEvaluator.Tests;

public class ExpressionEvaluationServiceTests
{
    private readonly ExpressionEvaluationService _sut = new(
        new BruteForceExpressionEvaluator(), new OptimizedExpressionEvaluator());

    [Theory]
    [MemberData(nameof(ExpressionEvaluatorTheoryData.ValidExpressions), MemberType = typeof(ExpressionEvaluatorTheoryData))]
    public void Compare_ValidExpressions_BothAlgorithmsMatchExpected(string expression, double expected)
    {
        var (bruteForce, optimized) = _sut.Compare(expression);

        Assert.Equal(expected, bruteForce, precision: 9);
        Assert.Equal(expected, optimized, precision: 9);
    }
}
