namespace API.Services;

public interface IBookmarkService
{
    Task<bool> SaveAsync(int userId, int bookmarkedUserId, CancellationToken ct = default);
    Task<bool> RemoveAsync(int userId, int bookmarkedUserId, CancellationToken ct = default);
}
