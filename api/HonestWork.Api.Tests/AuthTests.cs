using System.Net;
using System.Net.Http.Json;
using HonestWork.Api.Data;
using HonestWork.Api.Features.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HonestWork.Api.Tests;

public class AuthTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    readonly HttpClient client = factory.CreateClient();

    [Fact]
    public async Task Signs_in_with_the_right_password_and_remembers_the_user()
    {
        var response = await client.PostAsJsonAsync("/api/auth/login", new LoginRequest("Test", "test"));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var me = await client.GetFromJsonAsync<LoginResponse>("/api/auth/me");
        Assert.Equal("Test", me!.Username);
    }

    [Theory]
    [InlineData("Test", "wrong")]
    [InlineData("Nobody", "wrong")]
    public async Task Rejects_a_wrong_username_or_password_without_telling_which(string username, string password)
    {
        var error = await LoginError(new LoginRequest(username, password));

        Assert.Equal("Wrong username or password.", error);
    }

    [Theory]
    [InlineData("Test")]
    [InlineData("Nobody")]
    public async Task Names_the_demo_user_whose_password_was_typed(string username)
    {
        var error = await LoginError(new LoginRequest(username, "admin"));

        Assert.Equal("Hey, this password is taken by 'Admin'.", error);
    }

    [Fact]
    public async Task Failed_sign_in_starts_no_session()
    {
        var visitor = factory.CreateClient();
        await visitor.PostAsJsonAsync("/api/auth/login", new LoginRequest("Test", "wrong"));

        var response = await visitor.GetAsync("/api/auth/me");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Recovery_hints_the_demo_password_for_a_loosely_typed_username()
    {
        var recovered = await Recover("  tEsT ");

        Assert.Contains("'test'", recovered.Message);
        Assert.Null(recovered.Suggestions);
    }

    [Fact]
    public async Task Recovery_never_reveals_a_real_password()
    {
        var username = $"no-demo-{Guid.NewGuid():N}";
        await WithDatabase(database =>
        {
            database.Users.Add(new User { Id = Guid.NewGuid(), Username = username, PasswordHash = "hash" });

            return database.SaveChangesAsync();
        });

        try
        {
            var recovered = await Recover(username);

            Assert.Equal("Found you. We only store a hash of your password, so even we cannot read it.", recovered.Message);
            Assert.Null(recovered.Suggestions);
        }
        finally
        {
            await WithDatabase(database => database.Users.Where(user => user.Username == username).ExecuteDeleteAsync());
        }
    }

    [Fact]
    public async Task Recovery_suggests_the_three_closest_usernames_for_an_unknown_one()
    {
        var recovered = await Recover("Tset");

        Assert.Equal(3, recovered.Suggestions!.Length);
        Assert.Equal("Test", recovered.Suggestions[0]);
    }

    [Fact]
    public async Task Recovery_needs_at_least_a_bit_of_the_username()
    {
        var response = await client.PostAsJsonAsync("/api/auth/recover", new RecoverRequest("   "));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Assert.Contains("username", problem!.Errors.Keys);
    }

    async Task<string> LoginError(LoginRequest request)
    {
        var response = await client.PostAsJsonAsync("/api/auth/login", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        return Assert.Single(problem!.Errors["password"]);
    }

    async Task<RecoverResponse> Recover(string username)
    {
        var response = await client.PostAsJsonAsync("/api/auth/recover", new RecoverRequest(username));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        return (await response.Content.ReadFromJsonAsync<RecoverResponse>())!;
    }

    async Task WithDatabase(Func<HonestWorkDbContext, Task> work)
    {
        using var scope = factory.Services.CreateScope();
        await work(scope.ServiceProvider.GetRequiredService<HonestWorkDbContext>());
    }
}
