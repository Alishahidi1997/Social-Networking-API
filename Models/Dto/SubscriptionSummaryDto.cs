namespace API.Models.Dto;

public record SubscriptionSummaryDto
{
    public int PlanId { get; init; }
    public required string PlanName { get; init; }
    public bool UnlimitedLikes { get; init; }
    public bool SeeWhoLikedYou { get; init; }
    public bool PriorityInDiscovery { get; init; }
    public DateTime? SubscriptionExpiresUtc { get; init; }
    public bool IsPaidPlanActive { get; init; }
    public bool AutoRenew { get; init; }
    public int RenewalDays { get; init; }
}
