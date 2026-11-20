using ExpressionEvaluator.Core;
using Xunit;

namespace ExpressionEvaluator.Tests;

public class EvaluationTraceTests
{
    [Theory]
    [InlineData("4+5*2", 14d)]
    [InlineData("4+5/2-1", 5.5d)]
    public void BruteForce_Trace_EndsAtCorrectResultWithMultipleSteps(string expression, double expected)
    {
        var trace = new BruteForceExpressionEvaluator().EvaluateWithTrace(expression);

        Assert.Equal(expected, trace.Result, precision: 9);
        Assert.True(trace.Steps.Count > 1);
    }

    [Theory]
    [InlineData("4+5*2", 14d)]
    [InlineData("4+5/2-1", 5.5d)]
    public void Optimized_Trace_EndsAtCorrectResultWithOneStepPerToken(string expression, double expected)
    {
        var trace = new OptimizedExpressionEvaluator().EvaluateWithTrace(expression);

        Assert.Equal(expected, trace.Result, precision: 9);
        Assert.True(trace.Steps.Count > 1);
    }
}
