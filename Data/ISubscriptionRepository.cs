using API.Entities;

namespace API.Data;

public interface ISubscriptionRepository
{
    Task<IReadOnlyList<SubscriptionPlan>> GetAllPlansAsync(CancellationToken ct = default);
    Task<SubscriptionPlan?> GetPlanByIdAsync(int id, CancellationToken ct = default);
}
