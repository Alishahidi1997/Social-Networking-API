using API.Data;
using API.Entities;

namespace API.Services;

public class LikesService(ILikesRepository likesRepo, IUserRepository userRepo) : ILikesService
{
    private const int MaxLikesPerUtcDay = 20;

    public async Task<LikeAddResult> AddLikeAsync(int sourceUserId, int targetUserId, CancellationToken ct = default)
    {
        if (sourceUserId == targetUserId)
            return LikeAddResult.InvalidTarget;

        var targetUser = await userRepo.GetUserByIdAsync(targetUserId, ct);
        if (targetUser == null)
            return LikeAddResult.InvalidTarget;

        if (await likesRepo.GetUserLikeAsync(sourceUserId, targetUserId, ct) != null)
            return LikeAddResult.AlreadyLiked;

        var utcDayStart = DateTime.UtcNow.Date;
        var sentToday = await likesRepo.CountLikesSentOnUtcDayAsync(sourceUserId, utcDayStart, ct);
        if (sentToday >= MaxLikesPerUtcDay)
            return LikeAddResult.DailyLimitReached;

        var like = new UserLike
        {
            SourceUserId = sourceUserId,
            TargetUserId = targetUserId,
            LikedAt = DateTime.UtcNow
        };
        likesRepo.AddLike(like);

        return await userRepo.SaveAllAsync(ct) ? LikeAddResult.Success : LikeAddResult.Failed;
    }
}
