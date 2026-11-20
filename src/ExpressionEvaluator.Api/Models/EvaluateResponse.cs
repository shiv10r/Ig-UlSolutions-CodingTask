namespace ExpressionEvaluator.Api.Models;

public class EvaluateResponse
{
    public string Expression { get; set; } = string.Empty;
    public double Result { get; set; }
    public string Algorithm { get; set; } = string.Empty;
}

public class EvaluateCompareResponse
{
    public string Expression { get; set; } = string.Empty;
    public double BruteForceResult { get; set; }
    public double OptimizedResult { get; set; }
    public bool Match { get; set; }
}
