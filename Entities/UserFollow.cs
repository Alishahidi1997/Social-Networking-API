namespace API.Entities;

public class UserFollow
{
    public int FollowerId { get; set; }
    public AppUser Follower { get; set; } = null!;

    public int FollowingId { get; set; }
    public AppUser Following { get; set; } = null!;

    public DateTime FollowedAtUtc { get; set; }
}
