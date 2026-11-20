using System.Collections.Concurrent;
using System.Diagnostics;
using HonestWork.Api.Data;
using HonestWork.Api.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Options;

namespace HonestWork.Api.Features.Cache;

public class CacheService(IOptions<CacheOptions> options, IServiceScopeFactory scopes, HybridCache cache, ILogger<CacheService> logger)
{
    private const string SalesQuery = """
        SELECT c."Name" AS "Category", COUNT(*) AS "Orders", SUM(o."Quantity"::bigint * p."PriceCents") AS "Revenue"
        FROM "SalesOrders" o
        JOIN "SalesProducts" p ON p."Id" = o."ProductId"
        JOIN "SalesCategories" c ON c."Id" = p."CategoryId"
        GROUP BY c."Name"
        ORDER BY "Revenue" DESC
        """;

    private readonly CacheOptions settings = options.Value;
    private readonly ConcurrentDictionary<Guid, CacheSession> sessions = new();
    private readonly SemaphoreSlim queryLimit = new(options.Value.MaxConcurrentQueries);
    private readonly HybridCacheEntryOptions entryOptions = new()
    {
        Expiration = TimeSpan.FromSeconds(options.Value.EntrySeconds),
        LocalCacheExpiration = TimeSpan.FromSeconds(options.Value.EntrySeconds),
        Flags = HybridCacheEntryFlags.DisableDistributedCache,
    };

    public async Task<Result<CacheResponse>> Query(Guid sessionId, CancellationToken cancellationToken)
    {
        await RemoveIdleSessions();

        if (!sessions.ContainsKey(sessionId) && sessions.Count >= settings.MaxSessions)
        {
            logger.SessionLimitReached(settings.MaxSessions);

            return new ValidationError("session", "Too many demo sessions are open. Try again in a moment.");
        }

        var started = Stopwatch.GetTimestamp();
        var session = sessions.GetOrAdd(sessionId, _ => new CacheSession(Environment.TickCount64));
        var recorded = session.Record(Environment.TickCount64, settings);
        var metrics = recorded.Metrics;

        if (recorded.Switched)
        {
            LogCacheSwitch(sessionId, metrics);
        }

        var queried = false;

        try
        {
            var rows = metrics.CacheActive
                ? await cache.GetOrCreateAsync(KeyOf(sessionId), async token =>
                {
                    queried = true;

                    return await QuerySales(session, token);
                }, entryOptions, cancellationToken: cancellationToken)
                : await QuerySales(session, cancellationToken);
            queried |= !metrics.CacheActive;

            var latency = Stopwatch.GetElapsedTime(started).TotalMilliseconds;
            var cacheStatus = metrics.CacheActive ? (queried ? "miss" : "hit") : "disabled";

            var source = queried ? "postgresql" : "cache";
            logger.ReportServed(sessionId, source, cacheStatus, latency);

            return new CacheResponse(source, cacheStatus, latency, metrics, rows);
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            logger.QueryTimedOut(sessionId, settings.QueryTimeoutSeconds);

            return new ValidationError("query", "The database is busy. Try again in a moment.");
        }
    }

    public async Task Reset(Guid sessionId)
    {
        sessions.TryRemove(sessionId, out _);
        await cache.RemoveAsync(KeyOf(sessionId));
    }

    private static string KeyOf(Guid sessionId)
    {
        return $"sales:{sessionId}";
    }

    private async Task RemoveIdleSessions()
    {
        var idleSince = Environment.TickCount64 - settings.SessionIdleSeconds * 1000L;
        var idle = sessions.Where(entry => entry.Value.LastSeen < idleSince).Select(entry => entry.Key).ToList();

        foreach (var sessionId in idle)
        {
            await Reset(sessionId);
        }

        if (idle.Count > 0)
        {
            logger.IdleSessionsEvicted(idle.Count);
        }
    }

    private void LogCacheSwitch(Guid sessionId, CacheMetrics metrics)
    {
        if (metrics.CacheActive)
        {
            logger.CacheEnabled(sessionId, metrics.Rps);
            return;
        }

        logger.CacheDisabled(sessionId, metrics.Rps);
    }

    private async Task<SalesRow[]> QuerySales(CacheSession session, CancellationToken cancellationToken)
    {
        using var budget = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        budget.CancelAfter(TimeSpan.FromSeconds(settings.QueryTimeoutSeconds));
        await queryLimit.WaitAsync(budget.Token);

        try
        {
            using var scope = scopes.CreateScope();
            var database = scope.ServiceProvider.GetRequiredService<HonestWorkDbContext>();
            session.CountQuery();

            return await database.Database.SqlQueryRaw<SalesRow>(SalesQuery).ToArrayAsync(budget.Token);
        }
        finally
        {
            queryLimit.Release();
        }
    }
}
