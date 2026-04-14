namespace API.Services;

public class LoggingEmailSender(ILogger<LoggingEmailSender> logger) : IEmailSender
{
    public Task SendEmailAsync(string toAddress, string subject, string textBody, string? htmlBody = null, CancellationToken ct = default)
    {
        logger.LogInformation(
            "Email (no SMTP configured) → To: {To}\nSubject: {Subject}\n{Body}",
            toAddress, subject, textBody);
        return Task.CompletedTask;
    }
}
