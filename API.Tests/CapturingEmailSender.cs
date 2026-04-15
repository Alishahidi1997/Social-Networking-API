using API.Services;

namespace API.Tests;

public sealed class CapturingEmailSender : IEmailSender
{
    public string? LastTo { get; private set; }
    public string? LastSubject { get; private set; }
    public string? LastBody { get; private set; }

    public Task SendEmailAsync(string toAddress, string subject, string textBody, string? htmlBody = null, CancellationToken ct = default)
    {
        LastTo = toAddress;
        LastSubject = subject;
        LastBody = textBody;
        return Task.CompletedTask;
    }

    public string? GetLastConfirmationToken()
    {
        if (LastBody == null) return null;
        const string marker = "CONFIRMATION_TOKEN:";
        var i = LastBody.IndexOf(marker, StringComparison.Ordinal);
        if (i < 0) return null;
        return LastBody[(i + marker.Length)..].Trim();
    }

    public string? GetLastResetPasswordToken()
    {
        if (LastBody == null) return null;
        const string marker = "RESET_PASSWORD_TOKEN:";
        var i = LastBody.IndexOf(marker, StringComparison.Ordinal);
        if (i < 0) return null;
        return LastBody[(i + marker.Length)..].Trim();
    }

    public void Clear()
    {
        LastTo = null;
        LastSubject = null;
        LastBody = null;
    }
}
