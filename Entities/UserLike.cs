namespace API.Entities;

public class UserLike
{
    public int SourceUserId { get; set; }
    public AppUser SourceUser { get; set; } = null!;

    public int TargetUserId { get; set; }
    public AppUser TargetUser { get; set; } = null!;

    /// <summary>UTC time the source user sent this like (used for daily limits).</summary>
    public DateTime LikedAt { get; set; }
}
