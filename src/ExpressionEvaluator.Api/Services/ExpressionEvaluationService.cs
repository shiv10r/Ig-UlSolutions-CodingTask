using ExpressionEvaluator.Core;
using Microsoft.Extensions.DependencyInjection;

namespace ExpressionEvaluator.Api.Services;

public class ExpressionEvaluationService(
    [FromKeyedServices("bruteForce")] IExpressionEvaluator bruteForceEvaluator,
    [FromKeyedServices("optimized")] IExpressionEvaluator optimizedEvaluator) : IExpressionEvaluationService
{
    public double EvaluateOptimized(string expression) => optimizedEvaluator.Evaluate(expression);

    public double EvaluateBruteForce(string expression) => bruteForceEvaluator.Evaluate(expression);

    public (double BruteForce, double Optimized) Compare(string expression)
        => (bruteForceEvaluator.Evaluate(expression), optimizedEvaluator.Evaluate(expression));

    public (EvaluationTrace BruteForce, EvaluationTrace Optimized) Trace(string expression)
        => (bruteForceEvaluator.EvaluateWithTrace(expression), optimizedEvaluator.EvaluateWithTrace(expression));
}
