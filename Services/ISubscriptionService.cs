using API.Models.Dto;

namespace API.Services;

public interface ISubscriptionService
{
    Task<IReadOnlyList<SubscriptionPlanDto>> GetPlansAsync(CancellationToken ct = default);
    Task<SubscriptionSummaryDto?> GetMySummaryAsync(int userId, CancellationToken ct = default);
    Task<bool> SubscribeAsync(int userId, SubscribeDto dto, CancellationToken ct = default);
    Task<bool> CancelAsync(int userId, CancellationToken ct = default);
    Task<bool> SetAutoRenewAsync(int userId, bool enabled, CancellationToken ct = default);
    Task ReconcileUserAsync(int userId, CancellationToken ct = default);
}
