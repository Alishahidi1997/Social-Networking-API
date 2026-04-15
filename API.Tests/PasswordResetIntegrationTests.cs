using System.Net;
using System.Net.Http.Json;
using API.Models.Dto;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace API.Tests;

public class PasswordResetIntegrationTests : IClassFixture<ApiWebApplicationFactory>, IDisposable
{
    private readonly ApiWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public PasswordResetIntegrationTests(ApiWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    public void Dispose() => _client.Dispose();

    private static string UniqueName() => $"r{Guid.NewGuid():N}"[..14];

    private CapturingEmailSender GetSender() => _factory.Services.GetRequiredService<CapturingEmailSender>();

    [Fact]
    public async Task Forgot_password_sends_token_and_reset_changes_login_password()
    {
        GetSender().Clear();
        var name = UniqueName();
        await _client.PostAsJsonAsync("/api/account/register", new RegisterDto
        {
            UserName = name,
            Email = $"{name}@test.com",
            Password = "Aa123456"
        });

        var forgot = await _client.PostAsJsonAsync("/api/account/forgot-password", new ForgotPasswordDto
        {
            Email = $"{name}@test.com"
        });
        Assert.Equal(HttpStatusCode.NoContent, forgot.StatusCode);

        var token = GetSender().GetLastResetPasswordToken();
        Assert.False(string.IsNullOrWhiteSpace(token));

        var reset = await _client.PostAsJsonAsync("/api/account/reset-password", new ResetPasswordDto
        {
            Token = token!,
            NewPassword = "Bb123456"
        });
        Assert.Equal(HttpStatusCode.NoContent, reset.StatusCode);

        var oldLogin = await _client.PostAsJsonAsync("/api/account/login", new LoginDto
        {
            UserName = name,
            Password = "Aa123456"
        });
        Assert.Equal(HttpStatusCode.Unauthorized, oldLogin.StatusCode);

        var newLogin = await _client.PostAsJsonAsync("/api/account/login", new LoginDto
        {
            UserName = name,
            Password = "Bb123456"
        });
        Assert.Equal(HttpStatusCode.OK, newLogin.StatusCode);
    }

    [Fact]
    public async Task Forgot_password_unknown_email_still_returns_no_content()
    {
        GetSender().Clear();
        var res = await _client.PostAsJsonAsync("/api/account/forgot-password", new ForgotPasswordDto
        {
            Email = "missing-user@test.com"
        });

        Assert.Equal(HttpStatusCode.NoContent, res.StatusCode);
        Assert.Null(GetSender().GetLastResetPasswordToken());
    }

    [Fact]
    public async Task Reset_password_with_invalid_token_returns_bad_request()
    {
        var res = await _client.PostAsJsonAsync("/api/account/reset-password", new ResetPasswordDto
        {
            Token = "not-a-valid-token",
            NewPassword = "Aa123456"
        });

        Assert.Equal(HttpStatusCode.BadRequest, res.StatusCode);
    }
}
