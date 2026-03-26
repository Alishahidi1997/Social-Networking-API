namespace API.Entities;

public class UserHobby
{
    public int AppUserId { get; set; }
    public AppUser AppUser { get; set; } = null!;

    public int HobbyId { get; set; }
    public Hobby Hobby { get; set; } = null!;
}
