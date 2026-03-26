namespace API.Models.Dto;

public record HobbyDto
{
    public int Id { get; init; }
    public required string Name { get; init; }
}
