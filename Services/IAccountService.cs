using API.Models.Dto;

namespace API.Services;

public interface IAccountService
{
    Task<(UserDto? User, string? Token)?> RegisterAsync(RegisterDto dto, CancellationToken ct = default);
    Task<(UserDto? User, string? Token)?> LoginAsync(LoginDto dto, CancellationToken ct = default);
    Task<bool> DeleteAccountAsync(int userId, CancellationToken ct = default);
    Task<ConfirmEmailResult> ConfirmEmailAsync(string token, CancellationToken ct = default);
    Task<bool> ResendConfirmationEmailAsync(int userId, CancellationToken ct = default);
    Task ForgotPasswordAsync(string email, CancellationToken ct = default);
    Task<ResetPasswordResult> ResetPasswordAsync(string token, string newPassword, CancellationToken ct = default);
}
