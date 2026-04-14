using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;

namespace API.Services;

public sealed class EmailConfirmationTokenService(IConfiguration config)
{
    private static readonly TimeSpan Lifetime = TimeSpan.FromHours(48);
    private const char Sep = '\x1f';

    private byte[] SigningKey()
    {
        var raw = config["EmailConfirmation:SigningKey"] ?? config["TokenKey"]
            ?? throw new InvalidOperationException("TokenKey or EmailConfirmation:SigningKey must be set.");
        return SHA256.HashData(Encoding.UTF8.GetBytes(raw));
    }

    public string CreateToken(int userId, string email, string userName)
    {
        var expires = DateTimeOffset.UtcNow.Add(Lifetime).ToUnixTimeSeconds();
        var payload = $"1{Sep}{userId}{Sep}{expires}{Sep}{email}{Sep}{userName}";
        var payloadBytes = Encoding.UTF8.GetBytes(payload);
        using var hmac = new HMACSHA256(SigningKey());
        var sig = hmac.ComputeHash(payloadBytes);
        return WebEncoders.Base64UrlEncode(payloadBytes) + "." + WebEncoders.Base64UrlEncode(sig);
    }

    public bool TryValidate(string token, out int userId, out string email, out string userName)
    {
        userId = 0;
        email = "";
        userName = "";
        try
        {
            var dot = token.IndexOf('.');
            if (dot <= 0 || dot >= token.Length - 1) return false;
            var payloadBytes = WebEncoders.Base64UrlDecode(token[..dot]);
            var sig = WebEncoders.Base64UrlDecode(token[(dot + 1)..]);
            using var hmac = new HMACSHA256(SigningKey());
            var expected = hmac.ComputeHash(payloadBytes);
            if (!CryptographicOperations.FixedTimeEquals(sig, expected)) return false;

            var payload = Encoding.UTF8.GetString(payloadBytes);
            var parts = payload.Split(Sep);
            if (parts.Length != 5 || parts[0] != "1") return false;
            if (!int.TryParse(parts[1], out userId)) return false;
            if (!long.TryParse(parts[2], out var exp)) return false;
            if (DateTimeOffset.UtcNow.ToUnixTimeSeconds() > exp) return false;
            email = parts[3];
            userName = parts[4];
            return true;
        }
        catch
        {
            return false;
        }
    }
}
