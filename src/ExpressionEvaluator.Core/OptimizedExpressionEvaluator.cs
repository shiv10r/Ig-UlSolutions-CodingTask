using System.Globalization;

namespace ExpressionEvaluator.Core;

/// <summary>
/// Single-pass O(n) time / O(n) space implementation (the classic "Basic Calculator II"
/// approach): walks the tokens once, keeping a stack of signed terms. "*" and "/" are
/// resolved immediately against the previous term; "+" and "-" just push the signed value.
/// The final result is the sum of the stack, so no backtracking or re-scanning is needed.
/// </summary>
public class OptimizedExpressionEvaluator : IExpressionEvaluator
{
    public double Evaluate(string expression) => EvaluateWithTrace(expression).Result;

    public EvaluationTrace EvaluateWithTrace(string expression)
    {
        var tokens = Tokenizer.Tokenize(expression);
        var steps = new List<EvaluationStep>();

        var terms = new List<double>();
        var pendingOp = '+';

        for (var i = 0; i < tokens.Count; i++)
        {
            var token = tokens[i];
            if (token.Type != TokenType.Number) continue;

            var value = token.Number;
            string description;

            switch (pendingOp)
            {
                case '+':
                    terms.Add(value);
                    description = $"Read {Fmt(value)}, pending op '+': push +{Fmt(value)} as a new term";
                    break;
                case '-':
                    terms.Add(-value);
                    description = $"Read {Fmt(value)}, pending op '-': push -{Fmt(value)} as a new term";
                    break;
                case '*':
                    var before = terms[^1];
                    terms[^1] *= value;
                    description = $"Read {Fmt(value)}, pending op '*': {Fmt(before)} * {Fmt(value)} = {Fmt(terms[^1])} (resolved immediately, no backtracking)";
                    break;
                case '/':
                    if (value == 0) throw new ExpressionException("Division by zero.");
                    var beforeDiv = terms[^1];
                    terms[^1] /= value;
                    description = $"Read {Fmt(value)}, pending op '/': {Fmt(beforeDiv)} / {Fmt(value)} = {Fmt(terms[^1])} (resolved immediately, no backtracking)";
                    break;
                default:
                    throw new ExpressionException($"Unsupported operator '{pendingOp}'.");
            }

            steps.Add(new EvaluationStep(steps.Count + 1, description, BuildState(terms)));

            var nextOpIndex = i + 1;
            if (nextOpIndex < tokens.Count)
                pendingOp = tokens[nextOpIndex].Operator;
        }

        var result = terms.Sum();
        steps.Add(new EvaluationStep(steps.Count + 1, $"Sum all terms = {Fmt(result)}", Fmt(result)));

        return new EvaluationTrace { Result = result, Steps = steps };
    }

    private static string BuildState(List<double> terms) => "[" + string.Join(", ", terms.Select(Fmt)) + "]";

    private static string Fmt(double value) => value % 1 == 0
        ? ((long)value).ToString(CultureInfo.InvariantCulture)
        : value.ToString(CultureInfo.InvariantCulture);
}
