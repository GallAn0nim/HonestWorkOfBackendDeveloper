using System.Net;
using System.Net.Http.Json;
using HonestWork.Api.Features.Auth;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HonestWork.Api.Tests;

public class SecurityTests(SignedInFactory factory) : IClassFixture<SignedInFactory>
{
    [Fact]
    public async Task Too_many_requests_from_one_client_are_turned_away()
    {
        var client = WithSetting("RateLimiting:General:PermitLimit", "1");

        await client.GetAsync("/api/sort/algorithms");
        var response = await client.GetAsync("/api/sort/algorithms");

        Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);
    }

    [Fact]
    public async Task Repeated_sign_in_attempts_are_turned_away()
    {
        var client = WithSetting("RateLimiting:Auth:PermitLimit", "1");

        await client.PostAsJsonAsync("/api/auth/login", new LoginRequest("Test", "wrong"));
        var response = await client.PostAsJsonAsync("/api/auth/login", new LoginRequest("Test", "test"));

        Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);
    }

    [Fact]
    public async Task Heavy_endpoints_have_their_own_tighter_limit()
    {
        var client = WithSetting("RateLimiting:Heavy:PermitLimit", "1");

        await client.GetAsync("/api/cv");
        var heavy = await client.GetAsync("/api/cv");
        var light = await client.GetAsync("/api/sort/algorithms");

        Assert.Equal(HttpStatusCode.TooManyRequests, heavy.StatusCode);
        Assert.Equal(HttpStatusCode.OK, light.StatusCode);
    }

    [Fact]
    public async Task One_client_cannot_hold_more_log_streams_than_allowed()
    {
        var client = WithSetting("RateLimiting:LogStreamConnections", "1");
        using var open = new CancellationTokenSource();
        using var first = await client.GetAsync("/api/observability/logs", HttpCompletionOption.ResponseHeadersRead, open.Token);

        var second = await client.GetAsync("/api/observability/logs", HttpCompletionOption.ResponseHeadersRead);

        Assert.Equal(HttpStatusCode.OK, first.StatusCode);
        Assert.Equal(HttpStatusCode.TooManyRequests, second.StatusCode);
        await open.CancelAsync();
    }

    HttpClient WithSetting(string key, string value)
    {
        return factory.WithWebHostBuilder(builder => builder.UseSetting(key, value)).CreateClient();
    }
}

public class ProductionTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task Production_redirects_plain_http_to_https()
    {
        var client = Production().CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

        var response = await client.GetAsync("/api/cv");

        Assert.Equal(HttpStatusCode.TemporaryRedirect, response.StatusCode);
        Assert.Equal("https", response.Headers.Location!.Scheme);
    }

    [Fact]
    public async Task Production_marks_the_session_cookie_secure()
    {
        var client = Production().CreateClient(new WebApplicationFactoryClientOptions { BaseAddress = new Uri("https://localhost") });

        var response = await client.PostAsJsonAsync("/api/auth/login", new LoginRequest("Test", "test"));

        var cookie = Assert.Single(response.Headers.GetValues("Set-Cookie"));
        Assert.Contains("secure", cookie, StringComparison.OrdinalIgnoreCase);
    }

    WebApplicationFactory<Program> Production()
    {
        var connectionString = factory.Services.GetRequiredService<IConfiguration>().GetConnectionString("HonestWork");

        return factory.WithWebHostBuilder(builder => builder
            .UseEnvironment("Production")
            .UseSetting("ConnectionStrings:HonestWork", connectionString));
    }
}
