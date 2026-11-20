using System.Text;

namespace ExpressionEvaluator.Core;

internal enum TokenType { Number, Operator }

internal readonly struct Token(TokenType type, double number = 0, char op = default)
{
    public TokenType Type { get; } = type;
    public double Number { get; } = number;
    public char Operator { get; } = op;
}

/// <summary>
/// Validates and tokenizes an expression. Centralized here so both evaluator
/// implementations share the same input-validation / security boundary.
/// </summary>
internal static class Tokenizer
{
    private const int MaxExpressionLength = 1000;
    private static readonly char[] ValidOperators = ['+', '-', '*', '/'];

    public static List<Token> Tokenize(string expression)
    {
        if (string.IsNullOrWhiteSpace(expression))
            throw new ExpressionException("Expression must not be empty.");

        if (expression.Length > MaxExpressionLength)
            throw new ExpressionException($"Expression exceeds maximum allowed length of {MaxExpressionLength} characters.");

        var tokens = new List<Token>();
        var i = 0;
        var expectNumber = true; // expression cannot start with an operator

        while (i < expression.Length)
        {
            var c = expression[i];

            if (char.IsWhiteSpace(c))
            {
                i++;
                continue;
            }

            if (char.IsDigit(c))
            {
                var start = i;
                var sb = new StringBuilder();
                while (i < expression.Length && char.IsDigit(expression[i]))
                {
                    sb.Append(expression[i]);
                    i++;
                }

                // Reject leading zeros like "007" to keep numbers unambiguous (still allow a lone "0").
                if (sb.Length > 1 && sb[0] == '0')
                    throw new ExpressionException($"Invalid number with leading zero at position {start}.");

                tokens.Add(new Token(TokenType.Number, number: double.Parse(sb.ToString())));
                expectNumber = false;
                continue;
            }

            if (Array.IndexOf(ValidOperators, c) >= 0)
            {
                if (expectNumber)
                    throw new ExpressionException($"Unexpected operator '{c}' at position {i}.");

                tokens.Add(new Token(TokenType.Operator, op: c));
                expectNumber = true;
                i++;
                continue;
            }

            throw new ExpressionException($"Invalid character '{c}' at position {i}. Only non-negative integers and + - * / are allowed.");
        }

        if (expectNumber)
            throw new ExpressionException("Expression must not end with an operator.");

        return tokens;
    }
}
