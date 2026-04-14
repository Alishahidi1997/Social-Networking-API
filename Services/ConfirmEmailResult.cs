namespace API.Services;

public enum ConfirmEmailResult
{
    Success,
    InvalidOrExpiredToken,
    AlreadyConfirmed,
    EmailMismatch
}
