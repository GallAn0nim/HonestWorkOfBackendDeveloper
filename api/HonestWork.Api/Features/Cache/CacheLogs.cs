namespace HonestWork.Api.Features.Cache;

internal static partial class CacheLogs
{
    [LoggerMessage(EventId = 4001, Level = LogLevel.Information, Message = "Cache enabled for session {SessionId} at {Rps} RPS")]
    public static partial void CacheEnabled(this ILogger logger, Guid sessionId, int rps);

    [LoggerMessage(EventId = 4002, Level = LogLevel.Information, Message = "Cache disabled for session {SessionId} at {Rps} RPS, reading PostgreSQL again")]
    public static partial void CacheDisabled(this ILogger logger, Guid sessionId, int rps);

    [LoggerMessage(EventId = 4003, Level = LogLevel.Debug, Message = "Sales report for session {SessionId} served from {Source} (cache {CacheStatus}, {LatencyMs:F1} ms)")]
    public static partial void ReportServed(this ILogger logger, Guid sessionId, string source, string cacheStatus, double latencyMs);

    [LoggerMessage(EventId = 4004, Level = LogLevel.Warning, Message = "Cache session rejected: {SessionLimit} sessions already open")]
    public static partial void SessionLimitReached(this ILogger logger, int sessionLimit);

    [LoggerMessage(EventId = 4005, Level = LogLevel.Warning, Message = "Sales query for session {SessionId} cancelled after its {TimeoutSeconds} s budget")]
    public static partial void QueryTimedOut(this ILogger logger, Guid sessionId, int timeoutSeconds);

    [LoggerMessage(EventId = 4006, Level = LogLevel.Information, Message = "Evicted {SessionCount} idle cache sessions")]
    public static partial void IdleSessionsEvicted(this ILogger logger, int sessionCount);
}
