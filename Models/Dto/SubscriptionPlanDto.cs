namespace API.Models.Dto;

public record SubscriptionPlanDto
{
    public int Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public decimal MonthlyPriceUsd { get; init; }
    public bool UnlimitedLikes { get; init; }
    public bool SeeWhoLikedYou { get; init; }
    public bool PriorityInDiscovery { get; init; }
}
