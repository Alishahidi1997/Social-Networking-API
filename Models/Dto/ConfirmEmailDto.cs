using System.ComponentModel.DataAnnotations;

namespace API.Models.Dto;

public record ConfirmEmailDto
{
    [Required]
    public required string Token { get; init; }
}
