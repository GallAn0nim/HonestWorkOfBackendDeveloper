using System.Net;
using System.Net.Http.Json;
using HonestWork.Api.Features.Cache;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace HonestWork.Api.Tests;

public class CacheServiceTests(SignedInFactory factory) : IClassFixture<SignedInFactory>
{
    [Fact]
    public async Task Slow_traffic_always_reads_the_database()
    {
        var client = factory.CreateClient();
        var sessionId = Guid.NewGuid();

        var first = await Query(client, sessionId);
        var second = await Query(client, sessionId);

        Assert.All([first, second], response =>
        {
            Assert.Equal("disabled", response.CacheStatus);
            Assert.Equal("postgresql", response.Source);
        });
        Assert.NotEmpty(second.Rows);
    }

    [Fact]
    public async Task Busy_traffic_queries_once_then_serves_the_cached_rows()
    {
        var client = WithSetting("Cache:ActivateAtRps", "1");
        var sessionId = Guid.NewGuid();

        var miss = await Query(client, sessionId);
        var hit = await Query(client, sessionId);

        Assert.Equal("miss", miss.CacheStatus);
        Assert.Equal("postgresql", miss.Source);
        Assert.Equal("hit", hit.CacheStatus);
        Assert.Equal("cache", hit.Source);
        Assert.Equal(1, hit.Metrics.Queries);
        Assert.Equal(miss.Rows, hit.Rows);
    }

    [Fact]
    public async Task Sessions_do_not_share_cached_rows()
    {
        var client = WithSetting("Cache:ActivateAtRps", "1");
        await Query(client, Guid.NewGuid());

        var other = await Query(client, Guid.NewGuid());

        Assert.Equal("miss", other.CacheStatus);
    }

    [Fact]
    public async Task Reset_drops_the_cached_rows_and_the_counters()
    {
        var client = WithSetting("Cache:ActivateAtRps", "1");
        var sessionId = Guid.NewGuid();
        await Query(client, sessionId);

        await client.PostAsync($"/api/cache/{sessionId}/reset", null);
        var afterReset = await Query(client, sessionId);

        Assert.Equal("miss", afterReset.CacheStatus);
        Assert.Equal(1, afterReset.Metrics.Requests);
    }

    [Fact]
    public async Task New_sessions_beyond_the_limit_are_turned_away()
    {
        var client = WithSetting("Cache:MaxSessions", "0");

        var response = await client.PostAsync($"/api/cache/{Guid.NewGuid()}/requests", null);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Assert.Contains("session", problem!.Errors.Keys);
    }

    HttpClient WithSetting(string key, string value)
    {
        return factory.WithWebHostBuilder(builder => builder.UseSetting(key, value)).CreateClient();
    }

    static async Task<CacheResponse> Query(HttpClient client, Guid sessionId)
    {
        var response = await client.PostAsync($"/api/cache/{sessionId}/requests", null);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        return (await response.Content.ReadFromJsonAsync<CacheResponse>())!;
    }
}
