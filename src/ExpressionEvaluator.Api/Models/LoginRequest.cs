using System.ComponentModel.DataAnnotations;

namespace ExpressionEvaluator.Api.Models;

public class LoginRequest
{
    [Required(AllowEmptyStrings = false)]
    public string Username { get; set; } = string.Empty;

    [Required(AllowEmptyStrings = false)]
    public string Password { get; set; } = string.Empty;
}
