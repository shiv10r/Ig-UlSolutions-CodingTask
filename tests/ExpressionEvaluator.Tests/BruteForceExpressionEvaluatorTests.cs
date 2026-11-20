using ExpressionEvaluator.Core;
using Xunit;

namespace ExpressionEvaluator.Tests;

public class BruteForceExpressionEvaluatorTests
{
    private readonly BruteForceExpressionEvaluator _sut = new();

    [Theory]
    [MemberData(nameof(ExpressionEvaluatorTheoryData.ValidExpressions), MemberType = typeof(ExpressionEvaluatorTheoryData))]
    public void Evaluate_ValidExpressions_ReturnsExpectedResult(string expression, double expected)
    {
        Assert.Equal(expected, _sut.Evaluate(expression), precision: 9);
    }

    [Theory]
    [MemberData(nameof(ExpressionEvaluatorTheoryData.InvalidExpressions), MemberType = typeof(ExpressionEvaluatorTheoryData))]
    public void Evaluate_InvalidExpressions_ThrowsExpressionException(string expression)
    {
        Assert.Throws<ExpressionException>(() => _sut.Evaluate(expression));
    }
}
