namespace API.Services;

public interface ILikesService
{
    Task<LikeAddResult> AddLikeAsync(int sourceUserId, int targetUserId, int? targetPhotoId, CancellationToken ct = default);
}
