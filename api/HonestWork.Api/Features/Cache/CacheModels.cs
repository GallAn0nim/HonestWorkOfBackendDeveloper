namespace HonestWork.Api.Features.Cache;

public record SalesRow(string Category, long Orders, long Revenue);
public record CacheMetrics(int Rps, bool CacheActive, long Requests, long Queries)
{
    public static readonly CacheMetrics Empty = new(0, false, 0, 0);
}
public record RecordedRequest(CacheMetrics Metrics, bool Switched);
public record CacheResponse(string Source, string CacheStatus, double LatencyMs, CacheMetrics Metrics, IReadOnlyList<SalesRow> Rows);
