using ExpressionEvaluator.Core;

namespace ExpressionEvaluator.Api.Services;

/// <summary>
/// Service layer between the controller and ExpressionEvaluator.Core: the
/// controller stays thin (HTTP concerns only), this orchestrates which
/// algorithm(s) to run.
/// </summary>
public interface IExpressionEvaluationService
{
    double EvaluateOptimized(string expression);
    double EvaluateBruteForce(string expression);
    (double BruteForce, double Optimized) Compare(string expression);

    (EvaluationTrace BruteForce, EvaluationTrace Optimized) Trace(string expression);
}
