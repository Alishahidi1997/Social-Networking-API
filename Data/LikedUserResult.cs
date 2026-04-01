using API.Entities;

namespace API.Data;

public sealed record LikedUserResult(AppUser Member, Photo? LikedPhoto);
