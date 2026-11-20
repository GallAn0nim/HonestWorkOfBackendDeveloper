using System.Net;
using System.Net.Http.Json;
using HonestWork.Api.Features.Auth;
using Microsoft.AspNetCore.Mvc.Testing;

namespace HonestWork.Api.Tests;

public class AuthGateTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    readonly HttpClient client = factory.CreateClient();

    [Theory]
    [InlineData("GET", "/api/sort/algorithms")]
    [InlineData("GET", "/api/sort/random")]
    [InlineData("GET", "/api/maze/algorithms")]
    [InlineData("POST", "/api/maze/race")]
    [InlineData("POST", "/api/race/atomic")]
    [InlineData("POST", "/api/cache/7f6b2c1e-3a4d-4f5e-9b8a-1c2d3e4f5a6b/requests")]
    [InlineData("GET", "/api/quotes/random")]
    [InlineData("GET", "/api/auth/me")]
    public async Task Rejects_modules_without_a_session(string method, string url)
    {
        var response = await client.SendAsync(new HttpRequestMessage(new HttpMethod(method), url));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Signing_out_without_a_session_still_succeeds()
    {
        var response = await client.PostAsync("/api/auth/logout", null);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
}

public class SignedInTests(SignedInFactory factory) : IClassFixture<SignedInFactory>
{
    readonly HttpClient client = factory.CreateClient();

    [Fact]
    public async Task Me_returns_the_signed_in_user()
    {
        var user = await client.GetFromJsonAsync<LoginResponse>("/api/auth/me");

        Assert.Equal(SignedInFactory.Username, user!.Username);
    }
}
