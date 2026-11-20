namespace ExpressionEvaluator.Core;

public interface IExpressionEvaluator
{
    /// <summary>
    /// Evaluates a string expression of non-negative integers and the operators +, -, *, /
    /// using normal operator precedence (brackets are not supported).
    /// </summary>
    /// <exception cref="ExpressionException">Thrown when the expression is malformed or
    /// an arithmetic error (e.g. division by zero) occurs.</exception>
    double Evaluate(string expression);

    /// <summary>
    /// Same evaluation, but also returns a human-readable, step-by-step trace of how
    /// the algorithm reduced the expression to its final result (for visualization).
    /// </summary>
    EvaluationTrace EvaluateWithTrace(string expression);
}
