using API.Entities;

namespace API.Data;

public interface IUserModerationRepository
{
    Task<bool> IsBlockedEitherWayAsync(int userIdA, int userIdB, CancellationToken ct = default);
    Task<bool> ExistsBlockAsync(int blockerId, int blockedId, CancellationToken ct = default);
    Task<bool> ExistsMuteAsync(int muterId, int mutedId, CancellationToken ct = default);
    void AddBlock(UserBlock block);
    void RemoveBlock(UserBlock block);
    Task<UserBlock?> GetBlockAsync(int blockerId, int blockedId, CancellationToken ct = default);
    void AddMute(UserMute mute);
    void RemoveMute(UserMute mute);
    Task<UserMute?> GetMuteAsync(int muterId, int mutedId, CancellationToken ct = default);
    Task RemoveFollowsBetweenAsync(int userIdA, int userIdB, CancellationToken ct = default);
    Task RemoveMutesBetweenAsync(int userIdA, int userIdB, CancellationToken ct = default);
}
