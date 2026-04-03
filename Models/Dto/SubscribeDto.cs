using System.ComponentModel.DataAnnotations;

namespace API.Models.Dto;

public record SubscribeDto
{
    [Range(2, 999, ErrorMessage = "Choose a paid plan (Plus or Premium).")]
    public int PlanId { get; init; }

    [Range(1, 3650)]
    public int DurationDays { get; init; } = 30;
}
