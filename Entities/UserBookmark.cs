namespace API.Entities;

public class UserBookmark
{
    public int UserId { get; set; }
    public AppUser User { get; set; } = null!;

    public int BookmarkedUserId { get; set; }
    public AppUser BookmarkedUser { get; set; } = null!;

    public DateTime SavedAtUtc { get; set; }
}
