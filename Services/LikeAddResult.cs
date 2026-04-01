namespace API.Services;

public enum LikeAddResult
{
    Success,
    AlreadyLiked,
    InvalidTarget,
    InvalidPhoto,
    DailyLimitReached,
    Failed
}
