namespace API.Services;

public interface IFollowService
{
    Task<FollowAddResult> FollowAsync(int followerId, int followingId, CancellationToken ct = default);
    Task<bool> UnfollowAsync(int followerId, int followingId, CancellationToken ct = default);
}
