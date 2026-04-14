namespace API.Services;

public interface IEmailSender
{
    Task SendEmailAsync(string toAddress, string subject, string textBody, string? htmlBody = null, CancellationToken ct = default);
}
