namespace API.Models.Dto;

public record LikedMemberDto
{
    public required UserDto Member { get; init; }
    public PhotoDto? LikedPhoto { get; init; }
}
