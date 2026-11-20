using System.Net;
using System.Net.Http.Json;
using HonestWork.Api.Features.Sorting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace HonestWork.Api.Tests;

public class RandomDataTests
{
    [Fact]
    public void Random_input_stays_in_range()
    {
        var input = SortingAlgorithms.RandomInput(1000, 10, 89, new Random(1));
        Assert.Equal(1000, input.Length);
        Assert.All(input, value => Assert.InRange(value, 10, 89));
    }
}

public class RandomEndpointTests(SignedInFactory factory) : IClassFixture<SignedInFactory>
{
    readonly HttpClient client = factory.CreateClient();

    record InputBody(int[] Input);

    [Fact]
    public async Task Sort_random_defaults_to_12_numbers()
    {
        var body = await client.GetFromJsonAsync<InputBody>("/api/sort/random");
        Assert.Equal(12, body!.Input.Length);
    }

    [Theory]
    [InlineData("/api/sort/random")]
    [InlineData("/api/maze/generation")]
    public async Task Random_data_is_never_cached(string url)
    {
        var response = await client.GetAsync(url);
        Assert.Equal("no-store", response.Headers.CacheControl?.ToString());
    }

    [Theory]
    [InlineData("/api/sort/random?count=0")]
    [InlineData("/api/sort/random?count=33")]
    [InlineData("/api/sort/random?count=abc")]
    [InlineData("/api/maze/generation?seed=abc")]
    public async Task Rejects_bad_sizes(string url)
    {
        Assert.Equal(HttpStatusCode.BadRequest, (await client.GetAsync(url)).StatusCode);
    }
}

public class OpenApiTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    readonly HttpClient client = factory.CreateClient();

    [Fact]
    public async Task Document_lists_every_endpoint()
    {
        var document = await client.GetStringAsync("/openapi/v1.json");
        Assert.All(["/api/sort", "/api/sort/random", "/api/sort/algorithms", "/api/maze", "/api/maze/generation", "/api/maze/algorithms"],
            path => Assert.Contains($"\"{path}\"", document));
    }

    [Fact]
    public async Task Swagger_ui_is_served()
    {
        var response = await client.GetAsync("/swagger/index.html");
        response.EnsureSuccessStatusCode();
    }
}
