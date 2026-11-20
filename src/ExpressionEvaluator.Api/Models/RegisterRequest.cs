using System.ComponentModel.DataAnnotations;

namespace ExpressionEvaluator.Api.Models;

public class RegisterRequest
{
    [Required(AllowEmptyStrings = false)]
    [StringLength(32, MinimumLength = 3)]
    public string Username { get; set; } = string.Empty;

    [Required(AllowEmptyStrings = false)]
    [StringLength(100, MinimumLength = 8)]
    public string Password { get; set; } = string.Empty;
}
