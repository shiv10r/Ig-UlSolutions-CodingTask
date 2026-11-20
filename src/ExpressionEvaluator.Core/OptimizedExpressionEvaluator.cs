namespace ExpressionEvaluator.Core;

/// <summary>
/// Single-pass O(n) time / O(n) space implementation (the classic "Basic Calculator II"
/// approach): walks the tokens once, keeping a stack of signed terms. "*" and "/" are
/// resolved immediately against the previous term; "+" and "-" just push the signed value.
/// The final result is the sum of the stack, so no backtracking or re-scanning is needed.
/// </summary>
public class OptimizedExpressionEvaluator : IExpressionEvaluator
{
    public double Evaluate(string expression)
    {
        var tokens = Tokenizer.Tokenize(expression);

        var stack = new Stack<double>();
        char pendingOp = '+';

        for (var i = 0; i < tokens.Count; i++)
        {
            var token = tokens[i];
            if (token.Type != TokenType.Number) continue;

            var value = token.Number;
            switch (pendingOp)
            {
                case '+':
                    stack.Push(value);
                    break;
                case '-':
                    stack.Push(-value);
                    break;
                case '*':
                    stack.Push(stack.Pop() * value);
                    break;
                case '/':
                    var top = stack.Pop();
                    if (value == 0) throw new ExpressionException("Division by zero.");
                    stack.Push(top / value);
                    break;
            }

            var nextOpIndex = i + 1;
            if (nextOpIndex < tokens.Count)
                pendingOp = tokens[nextOpIndex].Operator;
        }

        var result = 0.0;
        while (stack.Count > 0) result += stack.Pop();
        return result;
    }
}
