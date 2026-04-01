namespace API.Services;

public enum LikeAddResult
{
    Success,
    AlreadyLiked,
    InvalidTarget,
    DailyLimitReached,
    Failed
}
