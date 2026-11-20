using System.Net;
using System.Net.Http.Json;
using HonestWork.Api.Features.Race;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace HonestWork.Api.Tests;

public class RaceTests(SignedInFactory factory) : IClassFixture<SignedInFactory>
{
    readonly HttpClient client = factory.CreateClient();

    [Fact]
    public async Task Atomic_sells_the_only_comic_to_exactly_one_customer()
    {
        var result = await Race("/api/race/atomic");

        Assert.Equal(1, result.Success);
        Assert.Equal(19, result.Rejected);
        Assert.Equal(1, result.Orders);
        Assert.Equal(0, result.StockLeft);
        Assert.Single(result.Purchases, bought => bought);
    }

    [Fact]
    public async Task Unsafe_oversells_when_every_customer_reads_the_stock_first()
    {
        var result = await Race("/api/race/unsafe");

        Assert.True(result.Success > 1);
        Assert.Equal(result.Success, result.Orders);
        Assert.Equal(0, result.StockLeft);
    }

    [Fact]
    public async Task Queue_sells_the_only_comic_to_exactly_one_customer()
    {
        var queued = await client.PostAsync("/api/race/queue", null);
        var run = await queued.Content.ReadFromJsonAsync<QueuedRace>();
        var result = await Race($"/api/race/queue/{run!.RunId}/process");

        Assert.Equal(20, run.Queued);
        Assert.Equal(1, result.Success);
        Assert.Equal(1, result.Orders);
        Assert.Equal(0, result.StockLeft);
    }

    [Fact]
    public async Task A_queue_is_processed_only_once()
    {
        var queued = await client.PostAsync("/api/race/queue", null);
        var run = await queued.Content.ReadFromJsonAsync<QueuedRace>();
        await Race($"/api/race/queue/{run!.RunId}/process");

        var again = await client.PostAsync($"/api/race/queue/{run.RunId}/process", null);

        await AssertRejected(again, "runId");
    }

    [Fact]
    public async Task Processing_an_unknown_queue_is_rejected()
    {
        var response = await client.PostAsync($"/api/race/queue/{Guid.NewGuid()}/process", null);

        await AssertRejected(response, "runId");
    }

    [Fact]
    public async Task Races_beyond_the_concurrency_limit_are_turned_away()
    {
        var busy = WithSetting("Race:MaxConcurrentRaces", "0");

        var response = await busy.PostAsync("/api/race/atomic", null);

        await AssertRejected(response, "race");
    }

    [Fact]
    public async Task Queues_beyond_the_pending_limit_are_turned_away()
    {
        var busy = WithSetting("Race:MaxPendingQueues", "0");

        var response = await busy.PostAsync("/api/race/queue", null);

        await AssertRejected(response, "race");
    }

    async Task<RaceResult> Race(string url)
    {
        var response = await client.PostAsync(url, null);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        return (await response.Content.ReadFromJsonAsync<RaceResult>())!;
    }

    HttpClient WithSetting(string key, string value)
    {
        return factory.WithWebHostBuilder(builder => builder.UseSetting(key, value)).CreateClient();
    }

    static async Task AssertRejected(HttpResponseMessage response, string field)
    {
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Assert.Contains(field, problem!.Errors.Keys);
    }
}
