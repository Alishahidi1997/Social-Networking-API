using API.Data;
using API.Entities;

namespace API.Services;

public class BookmarkService(IBookmarkRepository bookmarkRepo, IUserRepository userRepo) : IBookmarkService
{
    public async Task<bool> SaveAsync(int userId, int bookmarkedUserId, CancellationToken ct = default)
    {
        if (userId == bookmarkedUserId)
            return false;

        if (await userRepo.GetUserByIdAsync(bookmarkedUserId, ct) == null)
            return false;

        if (await bookmarkRepo.GetBookmarkAsync(userId, bookmarkedUserId, ct) != null)
            return true;

        bookmarkRepo.AddBookmark(new UserBookmark
        {
            UserId = userId,
            BookmarkedUserId = bookmarkedUserId,
            SavedAtUtc = DateTime.UtcNow
        });

        return await userRepo.SaveAllAsync(ct);
    }

    public async Task<bool> RemoveAsync(int userId, int bookmarkedUserId, CancellationToken ct = default)
    {
        var row = await bookmarkRepo.GetBookmarkAsync(userId, bookmarkedUserId, ct);
        if (row == null)
            return false;

        bookmarkRepo.RemoveBookmark(row);
        return await userRepo.SaveAllAsync(ct);
    }
}
