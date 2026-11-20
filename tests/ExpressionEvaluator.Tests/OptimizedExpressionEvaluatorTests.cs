using ExpressionEvaluator.Core;
using Xunit;

namespace ExpressionEvaluator.Tests;

public class OptimizedExpressionEvaluatorTests
{
    private readonly OptimizedExpressionEvaluator _sut = new();

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

    [Fact]
    public void Evaluate_VeryLongExpression_ThrowsExpressionException()
    {
        var expression = string.Join("+", Enumerable.Repeat("1", 1000));
        Assert.Throws<ExpressionException>(() => _sut.Evaluate(expression));
    }
}
