namespace API.Entities;

public class UserLike
{
    public int SourceUserId { get; set; }
    public AppUser SourceUser { get; set; } = null!;

    public int TargetUserId { get; set; }
    public AppUser TargetUser { get; set; } = null!;

    /// <summary>Which of the target user's photos they were viewing when liking (optional).</summary>
    public int? TargetPhotoId { get; set; }
    public Photo? TargetPhoto { get; set; }

    /// <summary>UTC time the source user sent this like (used for daily limits).</summary>
    public DateTime LikedAt { get; set; }
}
