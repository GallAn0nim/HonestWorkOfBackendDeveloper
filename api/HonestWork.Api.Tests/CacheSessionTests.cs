using HonestWork.Api.Features.Cache;

namespace HonestWork.Api.Tests;

public class CacheSessionTests
{
    static readonly CacheOptions Limits = new();

    static CacheMetrics Record(CacheSession session, params long[] times)
    {
        return times.Select(at => session.Record(at, Limits).Metrics).ToArray()[^1];
    }

    [Fact]
    public void Uses_the_database_below_the_activation_rate()
    {
        var metrics = Record(new CacheSession(0), 0, 400, 800);
        Assert.Equal(3, metrics.Rps);
        Assert.False(metrics.CacheActive);
    }

    [Fact]
    public void Uses_the_cache_at_four_requests_within_one_second()
    {
        var metrics = Record(new CacheSession(0), 0, 100, 200, 300);
        Assert.Equal(4, metrics.Rps);
        Assert.True(metrics.CacheActive);
    }

    [Fact]
    public void Returns_to_the_database_as_soon_as_the_rate_drops()
    {
        var session = new CacheSession(0);
        Record(session, 0, 100, 200, 300);
        var metrics = Record(session, 1350);
        Assert.Equal(1, metrics.Rps);
        Assert.False(metrics.CacheActive);
    }

    [Fact]
    public void Reports_only_the_requests_that_switch_the_cache()
    {
        var session = new CacheSession(0);
        var switches = new long[] { 0, 100, 200, 300, 400, 1500 }.Select(at => session.Record(at, Limits).Switched);

        Assert.Equal([false, false, false, true, false, true], switches);
    }

    [Fact]
    public void Counts_every_request()
    {
        Assert.Equal(5, Record(new CacheSession(0), 0, 100, 200, 300, 5000).Requests);
    }
}
