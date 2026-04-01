namespace API.Models.Dto;

public record LikeUserDto
{
    /// <summary>ID of the target member's photo shown when they tapped like (omit to use their main photo).</summary>
    public int? PhotoId { get; init; }
}
