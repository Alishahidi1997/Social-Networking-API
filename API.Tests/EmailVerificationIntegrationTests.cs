using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using API.Models.Dto;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace API.Tests;

public class EmailVerificationIntegrationTests : IClassFixture<ApiWebApplicationFactory>, IDisposable
{
    private readonly ApiWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public EmailVerificationIntegrationTests(ApiWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    public void Dispose() => _client.Dispose();

    private static string UniqueName() => $"e{Guid.NewGuid():N}"[..14];

    private CapturingEmailSender GetSender() => _factory.Services.GetRequiredService<CapturingEmailSender>();

    [Fact]
    public async Task Register_sends_token_and_confirm_email_marks_confirmed()
    {
        GetSender().Clear();
        var name = UniqueName();
        var reg = await _client.PostAsJsonAsync("/api/account/register", new RegisterDto
        {
            UserName = name,
            Email = $"{name}@test.com",
            Password = "Aa123456"
        });
        Assert.Equal(HttpStatusCode.OK, reg.StatusCode);
        using (var doc = JsonDocument.Parse(await reg.Content.ReadAsStringAsync()))
        {
            Assert.False(doc.RootElement.GetProperty("user").GetProperty("emailConfirmed").GetBoolean());
        }

        var token = GetSender().GetLastConfirmationToken();
        Assert.False(string.IsNullOrEmpty(token));

        var confirm = await _client.PostAsJsonAsync("/api/account/confirm-email", new ConfirmEmailDto { Token = token });
        Assert.Equal(HttpStatusCode.NoContent, confirm.StatusCode);

        var login = await _client.PostAsJsonAsync("/api/account/login", new LoginDto
        {
            UserName = name,
            Password = "Aa123456"
        });
        login.EnsureSuccessStatusCode();
        using var loginDoc = JsonDocument.Parse(await login.Content.ReadAsStringAsync());
        Assert.True(loginDoc.RootElement.GetProperty("user").GetProperty("emailConfirmed").GetBoolean());
    }

    [Fact]
    public async Task Confirm_twice_is_idempotent()
    {
        GetSender().Clear();
        var name = UniqueName();
        await _client.PostAsJsonAsync("/api/account/register", new RegisterDto
        {
            UserName = name,
            Email = $"{name}@test.com",
            Password = "Aa123456"
        });
        var token = GetSender().GetLastConfirmationToken();
        Assert.NotNull(token);

        Assert.Equal(HttpStatusCode.NoContent, (await _client.PostAsJsonAsync("/api/account/confirm-email", new ConfirmEmailDto { Token = token })).StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, (await _client.PostAsJsonAsync("/api/account/confirm-email", new ConfirmEmailDto { Token = token })).StatusCode);
    }

    [Fact]
    public async Task Invalid_token_returns_bad_request()
    {
        var res = await _client.PostAsJsonAsync("/api/account/confirm-email", new ConfirmEmailDto { Token = "not-a-valid-token" });
        Assert.Equal(HttpStatusCode.BadRequest, res.StatusCode);
    }
}
