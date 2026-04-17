using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class UserModerationRepository(AppDbContext context) : IUserModerationRepository
{
    public Task<bool> IsBlockedEitherWayAsync(int userIdA, int userIdB, CancellationToken ct = default) =>
        context.UserBlocks.AnyAsync(
            b => (b.BlockerId == userIdA && b.BlockedId == userIdB) ||
                 (b.BlockerId == userIdB && b.BlockedId == userIdA),
            ct);

    public Task<bool> ExistsBlockAsync(int blockerId, int blockedId, CancellationToken ct = default) =>
        context.UserBlocks.AnyAsync(b => b.BlockerId == blockerId && b.BlockedId == blockedId, ct);

    public Task<bool> ExistsMuteAsync(int muterId, int mutedId, CancellationToken ct = default) =>
        context.UserMutes.AnyAsync(m => m.MuterId == muterId && m.MutedId == mutedId, ct);

    public void AddBlock(UserBlock block) => context.UserBlocks.Add(block);

    public void RemoveBlock(UserBlock block) => context.UserBlocks.Remove(block);

    public Task<UserBlock?> GetBlockAsync(int blockerId, int blockedId, CancellationToken ct = default) =>
        context.UserBlocks.FirstOrDefaultAsync(b => b.BlockerId == blockerId && b.BlockedId == blockedId, ct);

    public void AddMute(UserMute mute) => context.UserMutes.Add(mute);

    public void RemoveMute(UserMute mute) => context.UserMutes.Remove(mute);

    public Task<UserMute?> GetMuteAsync(int muterId, int mutedId, CancellationToken ct = default) =>
        context.UserMutes.FirstOrDefaultAsync(m => m.MuterId == muterId && m.MutedId == mutedId, ct);

    public async Task RemoveFollowsBetweenAsync(int userIdA, int userIdB, CancellationToken ct = default)
    {
        var rows = await context.UserFollows
            .Where(f =>
                (f.FollowerId == userIdA && f.FollowingId == userIdB) ||
                (f.FollowerId == userIdB && f.FollowingId == userIdA))
            .ToListAsync(ct);
        context.UserFollows.RemoveRange(rows);
    }

    public async Task RemoveMutesBetweenAsync(int userIdA, int userIdB, CancellationToken ct = default)
    {
        var rows = await context.UserMutes
            .Where(m =>
                (m.MuterId == userIdA && m.MutedId == userIdB) ||
                (m.MuterId == userIdB && m.MutedId == userIdA))
            .ToListAsync(ct);
        context.UserMutes.RemoveRange(rows);
    }
}
