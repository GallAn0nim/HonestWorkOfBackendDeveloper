namespace HonestWork.Api.Features.Observability;

public record LogEntry(long Id, DateTimeOffset Timestamp, string Level, string Module, string Message, string? TraceId, string? SpanId);
public record MetricsSnapshot(MetricsOverview Overview, double[] Rps, IReadOnlyList<EndpointMetrics> Endpoints);
public record MetricsOverview(long Requests, double Rps, double? AverageMs, double? P95Ms, double? ErrorRate);
public record TraceRun(Guid RunId, DateTimeOffset StartedAt, int Requests, int Finished);
public record TraceDetail(Guid RunId, IReadOnlyList<TraceSpan> Spans, IReadOnlyList<LogEntry> Logs);
public record TraceSpan(string TraceId, string SpanId, string? ParentSpanId, string? LinkedSpanId, string Name, string Source, int? Customer,
    DateTimeOffset Start, double? DurationMs, double? QueueWaitMs, string? Outcome, bool Failed);
public record EndpointMetrics(string Method, string Route, long Requests, double Rps, double? P50Ms, double? P95Ms, double? ErrorRate, double[]? RpsSeries);
