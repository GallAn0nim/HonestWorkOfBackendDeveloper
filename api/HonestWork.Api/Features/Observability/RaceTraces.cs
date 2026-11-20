using System.Diagnostics;
using HonestWork.Api.Features.Race;
using Microsoft.Extensions.Options;

namespace HonestWork.Api.Features.Observability;

public sealed class RaceTraces : IDisposable
{
    private const string HostingSource = "Microsoft.AspNetCore";

    private readonly LogStream logs;
    private readonly int runCapacity;
    private readonly ActivityListener listener;
    private readonly Dictionary<Guid, Dictionary<string, TraceSpan>> runs = [];
    private readonly Queue<Guid> arrival = new();
    private readonly Lock gate = new();

    public RaceTraces(IOptions<ObservabilityOptions> options, LogStream logs)
    {
        this.logs = logs;
        runCapacity = options.Value.TraceRunCapacity;
        listener = new ActivityListener
        {
            ShouldListenTo = source => source.Name is RaceTracing.SourceName or HostingSource,
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded,
            ActivityStarted = activity => Record(activity, false),
            ActivityStopped = activity => Record(activity, true),
        };
        ActivitySource.AddActivityListener(listener);
    }

    public IReadOnlyList<TraceRun> Runs()
    {
        lock (gate)
        {
            return arrival.Reverse()
                .Select(runId => Summarize(runId, runs[runId].Values))
                .ToList();
        }
    }

    public TraceDetail? Find(Guid runId)
    {
        List<TraceSpan> spans;

        lock (gate)
        {
            if (!runs.TryGetValue(runId, out var recorded))
            {
                return null;
            }

            spans = recorded.Values.OrderBy(span => span.Start).ToList();
        }

        var traceIds = spans.Select(span => span.TraceId).ToHashSet();

        return new TraceDetail(runId, spans, logs.ForTraces(traceIds));
    }

    public void Dispose()
    {
        listener.Dispose();
    }

    private void Record(Activity activity, bool stopped)
    {
        if (activity.GetTagItem(RaceTracing.RunIdTag) is not Guid runId)
        {
            return;
        }

        var span = new TraceSpan(
            activity.TraceId.ToHexString(),
            activity.SpanId.ToHexString(),
            activity.ParentSpanId == default ? null : activity.ParentSpanId.ToHexString(),
            activity.Links.Select(link => link.Context.SpanId.ToHexString()).FirstOrDefault(),
            activity.DisplayName,
            SourceOf(activity),
            activity.GetTagItem(RaceTracing.CustomerTag) as int?,
            new DateTimeOffset(activity.StartTimeUtc),
            stopped ? activity.Duration.TotalMilliseconds : null,
            activity.GetTagItem(RaceTracing.QueueWaitTag) as double?,
            activity.GetTagItem(RaceTracing.OutcomeTag) as string,
            activity.Status == ActivityStatusCode.Error);

        lock (gate)
        {
            SpansOf(runId)[span.SpanId] = span;
        }
    }

    private Dictionary<string, TraceSpan> SpansOf(Guid runId)
    {
        if (runs.TryGetValue(runId, out var spans))
        {
            return spans;
        }

        if (arrival.Count == runCapacity)
        {
            runs.Remove(arrival.Dequeue());
        }

        arrival.Enqueue(runId);
        spans = [];
        runs[runId] = spans;

        return spans;
    }

    private static TraceRun Summarize(Guid runId, IEnumerable<TraceSpan> spans)
    {
        var recorded = spans.ToList();
        var queued = recorded.Count(span => span.Name == RaceTracing.Enqueue);
        var finished = recorded.Count(span => span is { Name: RaceTracing.Work, DurationMs: not null });

        return new TraceRun(runId, recorded.Min(span => span.Start), queued, finished);
    }

    private static string SourceOf(Activity activity)
    {
        return activity.OperationName switch
        {
            RaceTracing.Enqueue => "QUEUE",
            RaceTracing.Work => "WORKER",
            RaceTracing.ReadStock or RaceTracing.Buy => "DB",
            _ => "API",
        };
    }
}
