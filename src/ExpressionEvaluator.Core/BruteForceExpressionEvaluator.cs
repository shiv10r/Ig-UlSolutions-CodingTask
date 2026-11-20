namespace ExpressionEvaluator.Core;

/// <summary>
/// Straightforward O(n^2) implementation: repeatedly scans the token list left-to-right,
/// collapsing the first "*" or "/" pair it finds, then repeats for "+" and "-" once no
/// higher-precedence operator remains. Simple to read/verify but re-scans the list once
/// per operator it folds.
/// </summary>
public class BruteForceExpressionEvaluator : IExpressionEvaluator
{
    public double Evaluate(string expression)
    {
        var tokens = Tokenizer.Tokenize(expression);

        var values = new List<double>();
        var ops = new List<char>();
        foreach (var token in tokens)
        {
            if (token.Type == TokenType.Number) values.Add(token.Number);
            else ops.Add(token.Operator);
        }

        ReducePass(values, ops, '*', '/');
        ReducePass(values, ops, '+', '-');

        return values[0];
    }

    private static void ReducePass(List<double> values, List<char> ops, char opA, char opB)
    {
        var i = 0;
        while (i < ops.Count)
        {
            if (ops[i] == opA || ops[i] == opB)
            {
                var left = values[i];
                var right = values[i + 1];
                var result = Apply(ops[i], left, right);

                values[i] = result;
                values.RemoveAt(i + 1);
                ops.RemoveAt(i);
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
}
