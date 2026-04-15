namespace API.Services;

public enum ResetPasswordResult
{
    Success,
    InvalidOrExpiredToken,
    EmailMismatch
}
