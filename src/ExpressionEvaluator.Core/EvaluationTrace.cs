namespace ExpressionEvaluator.Core;

public record EvaluationStep(int StepNumber, string Description, string State);

public class EvaluationTrace
{
    public double Result { get; init; }
    public List<EvaluationStep> Steps { get; init; } = [];
}
