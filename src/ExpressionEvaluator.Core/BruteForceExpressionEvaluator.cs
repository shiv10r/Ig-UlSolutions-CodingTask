using System.Globalization;

namespace ExpressionEvaluator.Core;

/// <summary>
/// Straightforward O(n^2) implementation: repeatedly scans the token list left-to-right,
/// collapsing the first "*" or "/" pair it finds, then repeats for "+" and "-" once no
/// higher-precedence operator remains. Simple to read/verify but re-scans the list once
/// per operator it folds.
/// </summary>
public class BruteForceExpressionEvaluator : IExpressionEvaluator
{
    public double Evaluate(string expression) => EvaluateWithTrace(expression).Result;

    public EvaluationTrace EvaluateWithTrace(string expression)
    {
        var tokens = Tokenizer.Tokenize(expression);

        var values = new List<double>();
        var ops = new List<char>();
        foreach (var token in tokens)
        {
            if (token.Type == TokenType.Number) values.Add(token.Number);
            else ops.Add(token.Operator);
        }

        var steps = new List<EvaluationStep>
        {
            new(0, "Tokenize expression", BuildState(values, ops))
        };

        ReducePass(values, ops, '*', '/', steps, "Resolve * and / (higher precedence)");
        ReducePass(values, ops, '+', '-', steps, "Resolve + and - (remaining pass)");

        return new EvaluationTrace { Result = values[0], Steps = steps };
    }

    private static void ReducePass(List<double> values, List<char> ops, char opA, char opB, List<EvaluationStep> steps, string passLabel)
    {
        var i = 0;
        var announced = false;
        while (i < ops.Count)
        {
            if (ops[i] == opA || ops[i] == opB)
            {
                if (!announced)
                {
                    steps.Add(new EvaluationStep(steps.Count, passLabel, BuildState(values, ops)));
                    announced = true;
                }

                var left = values[i];
                var right = values[i + 1];
                var op = ops[i];
                var result = Apply(op, left, right);

                values[i] = result;
                values.RemoveAt(i + 1);
                ops.RemoveAt(i);

                steps.Add(new EvaluationStep(
                    steps.Count,
                    $"{Fmt(left)} {op} {Fmt(right)} = {Fmt(result)}",
                    BuildState(values, ops)));
            }
            else
            {
                i++;
            }
        }
    }

    private static double Apply(char op, double left, double right) => op switch
    {
        '+' => left + right,
        '-' => left - right,
        '*' => left * right,
        '/' => right == 0 ? throw new ExpressionException("Division by zero.") : left / right,
        _ => throw new ExpressionException($"Unsupported operator '{op}'.")
    };

    private static string BuildState(List<double> values, List<char> ops)
    {
        if (values.Count == 0) return string.Empty;
        var parts = new List<string> { Fmt(values[0]) };
        for (var i = 0; i < ops.Count; i++)
        {
            parts.Add(ops[i].ToString());
            parts.Add(Fmt(values[i + 1]));
        }
        return string.Join(" ", parts);
    }

    private static string Fmt(double value) => value % 1 == 0
        ? ((long)value).ToString(CultureInfo.InvariantCulture)
        : value.ToString(CultureInfo.InvariantCulture);
}
