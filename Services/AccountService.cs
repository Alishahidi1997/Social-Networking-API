using API.Data;
using API.Entities;
using API.Models.Dto;

namespace API.Services;

public class AccountService(
    IUserRepository userRepo,
    ITokenService tokenService,
    ISubscriptionService subscriptionService,
    IEmailSender emailSender,
    EmailConfirmationTokenService confirmationTokens,
    IConfiguration configuration,
    ILogger<AccountService> logger) : IAccountService
{
    public async Task<(UserDto? User, string? Token)?> RegisterAsync(RegisterDto dto, CancellationToken ct = default)
    {
        if (await userRepo.GetUserByUsernameAsync(dto.UserName.ToLowerInvariant(), ct) != null)
            return null;

        if (await userRepo.GetUserByEmailAsync(dto.Email.ToLowerInvariant(), ct) != null)
            return null;

        var user = new AppUser
        {
            UserName = dto.UserName.ToLowerInvariant(),
            Email = dto.Email.ToLowerInvariant(),
            EmailConfirmed = false,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Bio = dto.Bio,
            KnownAs = dto.KnownAs ?? dto.UserName,
            City = dto.City,
            Country = dto.Country,
            JobTitle = dto.JobTitle
        };

        if (dto.HobbyIds.Count > 0)
        {
            var hobbies = await userRepo.GetHobbiesByIdsAsync(dto.HobbyIds, ct);
            user.UserHobbies = hobbies
                .Select(h => new UserHobby { HobbyId = h.Id })
                .ToList();
        }

        userRepo.Add(user);
        if (!await userRepo.SaveAllAsync(ct))
            return null;

        try
        {
            await SendConfirmationEmailAsync(user, ct);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Could not send confirmation email to {Email}", user.Email);
        }

        var userWithPhotos = await userRepo.GetUserByUsernameWithPhotosAsync(user.UserName, ct);
        var userDto = userWithPhotos == null ? UserService.MapToUserDto(user) : UserService.MapToUserDto(userWithPhotos);
        return (userDto, tokenService.CreateToken(user));
    }

    public async Task<(UserDto? User, string? Token)?> LoginAsync(LoginDto dto, CancellationToken ct = default)
    {
        var user = await userRepo.GetUserByUsernameWithPhotosAsync(dto.UserName.ToLowerInvariant(), ct);
        if (user == null) return null;

        if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            return null;

        user.LastActive = DateTime.UtcNow;
        userRepo.Update(user);
        await userRepo.SaveAllAsync(ct);

        await subscriptionService.ReconcileUserAsync(user.Id, ct);

        return (UserService.MapToUserDto(user), tokenService.CreateToken(user));
    }

    public async Task<bool> DeleteAccountAsync(int userId, CancellationToken ct = default)
    {
        var user = await userRepo.GetUserByIdAsync(userId, ct);
        if (user is null) return false;

        userRepo.Delete(user);
        return await userRepo.SaveAllAsync(ct);
    }

    public async Task<ConfirmEmailResult> ConfirmEmailAsync(string token, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(token) || !confirmationTokens.TryValidate(token, out var userId, out var email, out _))
            return ConfirmEmailResult.InvalidOrExpiredToken;

        var user = await userRepo.GetUserByIdAsync(userId, ct);
        if (user == null)
            return ConfirmEmailResult.InvalidOrExpiredToken;

        if (!string.Equals(user.Email, email, StringComparison.OrdinalIgnoreCase))
            return ConfirmEmailResult.EmailMismatch;

        if (user.EmailConfirmed)
            return ConfirmEmailResult.AlreadyConfirmed;

        user.EmailConfirmed = true;
        userRepo.Update(user);
        return await userRepo.SaveAllAsync(ct) ? ConfirmEmailResult.Success : ConfirmEmailResult.InvalidOrExpiredToken;
    }

    public async Task<bool> ResendConfirmationEmailAsync(int userId, CancellationToken ct = default)
    {
        var user = await userRepo.GetUserByIdAsync(userId, ct);
        if (user == null) return false;
        if (user.EmailConfirmed) return true;

        try
        {
            await SendConfirmationEmailAsync(user, ct);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Resend confirmation failed for user {UserId}", userId);
            return false;
        }

        return true;
    }

    private async Task SendConfirmationEmailAsync(AppUser user, CancellationToken ct)
    {
        var token = confirmationTokens.CreateToken(user.Id, user.Email, user.UserName);
        var apiBase = (configuration["App:PublicApiBaseUrl"] ?? "").TrimEnd('/');
        var subject = "Confirm your email";
        var confirmPath = string.IsNullOrEmpty(apiBase)
            ? "(your API base URL)/api/account/confirm-email"
            : $"{apiBase}/api/account/confirm-email";
        var text =
            $"Hi {user.KnownAs ?? user.UserName},\n\n" +
            "Confirm your email by sending a POST request to:\n" +
            $"{confirmPath}\n" +
            "with JSON body containing the token (property name: \"token\").\n\n" +
            "Token (copy the entire line after the colon):\n\n" +
            $"{token}\n\n" +
            "This confirmation expires in 48 hours.\n\n" +
            $"CONFIRMATION_TOKEN:{token}\n";

        await emailSender.SendEmailAsync(user.Email, subject, text, htmlBody: null, ct);
    }
}
