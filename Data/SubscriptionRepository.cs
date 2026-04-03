using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class SubscriptionRepository(AppDbContext context) : ISubscriptionRepository
{
    public async Task<IReadOnlyList<SubscriptionPlan>> GetAllPlansAsync(CancellationToken ct = default) =>
        await context.SubscriptionPlans.AsNoTracking().OrderBy(p => p.Id).ToListAsync(ct);

    public async Task<SubscriptionPlan?> GetPlanByIdAsync(int id, CancellationToken ct = default) =>
        await context.SubscriptionPlans.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id, ct);
}
