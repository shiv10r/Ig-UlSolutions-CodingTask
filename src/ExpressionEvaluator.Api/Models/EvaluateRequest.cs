using System.ComponentModel.DataAnnotations;

namespace ExpressionEvaluator.Api.Models;

public class EvaluateRequest
{
    [Required(AllowEmptyStrings = false)]
    [StringLength(1000, MinimumLength = 1)]
    public string Expression { get; set; } = string.Empty;
}
