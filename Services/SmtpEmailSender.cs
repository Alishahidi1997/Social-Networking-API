using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace API.Services;

public sealed class SmtpOptions
{
    public const string SectionName = "Smtp";
    public string Host { get; set; } = "";
    public int Port { get; set; } = 587;
    public string? User { get; set; }
    public string? Password { get; set; }
    public bool UseSsl { get; set; } = true;
}

public sealed class EmailOptions
{
    public const string SectionName = "Email";
    public string FromAddress { get; set; } = "noreply@localhost";
    public string FromName { get; set; } = "Social App";
}

public sealed class SmtpEmailSender(IOptions<EmailOptions> emailOpt, IOptions<SmtpOptions> smtpOpt, ILogger<SmtpEmailSender> logger) : IEmailSender
{
    public async Task SendEmailAsync(string toAddress, string subject, string textBody, string? htmlBody = null, CancellationToken ct = default)
    {
        var smtp = smtpOpt.Value;
        var from = emailOpt.Value;

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(from.FromName, from.FromAddress));
        message.To.Add(MailboxAddress.Parse(toAddress));
        message.Subject = subject;

        var builder = new BodyBuilder { TextBody = textBody };
        if (htmlBody != null)
            builder.HtmlBody = htmlBody;
        message.Body = builder.ToMessageBody();

        try
        {
            using var client = new SmtpClient();
            var secure = smtp.UseSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None;
            await client.ConnectAsync(smtp.Host, smtp.Port, secure, ct);
            if (!string.IsNullOrEmpty(smtp.User))
                await client.AuthenticateAsync(smtp.User, smtp.Password ?? "", ct);
            await client.SendAsync(message, ct);
            await client.DisconnectAsync(true, ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "SMTP send failed to {To}", toAddress);
            throw;
        }
    }
}
