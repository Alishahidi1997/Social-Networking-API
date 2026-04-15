using System.ComponentModel.DataAnnotations;

namespace API.Models.Dto;

public record ResetPasswordDto
{
    [Required]
    public required string Token { get; init; }

    [Required, MinLength(6), MaxLength(100)]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$",
        ErrorMessage = "Password must contain uppercase, lowercase, and number")]
    public required string NewPassword { get; init; }
}
