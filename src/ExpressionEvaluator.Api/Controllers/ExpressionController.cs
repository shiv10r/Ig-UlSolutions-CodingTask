using ExpressionEvaluator.Api.Models;
using ExpressionEvaluator.Api.Services;
using ExpressionEvaluator.Core;
using Microsoft.AspNetCore.Mvc;

namespace ExpressionEvaluator.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ExpressionController(
    ILogger<ExpressionController> logger,
    IExpressionEvaluationService evaluationService) : ControllerBase
{
    /// <summary>Evaluates an expression using the optimized (default) single-pass algorithm.</summary>
    [HttpPost("evaluate")]
    [ProducesResponseType(typeof(EvaluateResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Evaluate([FromBody] EvaluateRequest request)
        => EvaluateWith(request, evaluationService.EvaluateOptimized, "optimized");

    /// <summary>Evaluates an expression using the brute-force O(n^2) algorithm.</summary>
    [HttpPost("evaluate/brute-force")]
    [ProducesResponseType(typeof(EvaluateResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult EvaluateBruteForce([FromBody] EvaluateRequest request)
        => EvaluateWith(request, evaluationService.EvaluateBruteForce, "brute-force");

    /// <summary>Evaluates with both algorithms and reports whether they agree.</summary>
    [HttpPost("evaluate/compare")]
    [ProducesResponseType(typeof(EvaluateCompareResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Compare([FromBody] EvaluateRequest request)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        try
        {
            var (bruteForceResult, optimizedResult) = evaluationService.Compare(request.Expression);

            return Ok(new EvaluateCompareResponse
            {
                Expression = request.Expression,
                BruteForceResult = bruteForceResult,
                OptimizedResult = optimizedResult,
                Match = Math.Abs(bruteForceResult - optimizedResult) < 1e-9
            });
        }
        catch (ExpressionException ex)
        {
            logger.LogInformation("Rejected expression: {Reason}", ex.Message);
            return Problem(detail: ex.Message, statusCode: StatusCodes.Status400BadRequest, title: "Invalid expression");
        }
    }

    /// <summary>Returns a human-readable, step-by-step trace of both algorithms for visualization.</summary>
    [HttpPost("evaluate/trace")]
    [ProducesResponseType(typeof(TraceResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Trace([FromBody] EvaluateRequest request)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        try
        {
            var (bruteForce, optimized) = evaluationService.Trace(request.Expression);

            return Ok(new TraceResponse
            {
                Expression = request.Expression,
                BruteForceSteps = bruteForce.Steps,
                OptimizedSteps = optimized.Steps,
                Result = optimized.Result
            });
        }
        catch (ExpressionException ex)
        {
            logger.LogInformation("Rejected expression: {Reason}", ex.Message);
            return Problem(detail: ex.Message, statusCode: StatusCodes.Status400BadRequest, title: "Invalid expression");
        }
    }

    private IActionResult EvaluateWith(EvaluateRequest request, Func<string, double> evaluate, string algorithmName)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        try
        {
            var result = evaluate(request.Expression);
            return Ok(new EvaluateResponse
            {
                Expression = request.Expression,
                Result = result,
                Algorithm = algorithmName
            });
        }
        catch (ExpressionException ex)
        {
            logger.LogInformation("Rejected expression: {Reason}", ex.Message);
            return Problem(detail: ex.Message, statusCode: StatusCodes.Status400BadRequest, title: "Invalid expression");
        }
    }
}
