using System.ComponentModel.DataAnnotations;

namespace API.Models.Dto;

public record ForgotPasswordDto
{
    [Required, EmailAddress]
    public required string Email { get; init; }
}
