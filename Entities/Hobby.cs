namespace API.Entities;

public class Hobby
{
    public int Id { get; set; }
    public required string Name { get; set; }

    public ICollection<UserHobby> UserHobbies { get; set; } = [];
}
