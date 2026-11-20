using System.Net.ServerSentEvents;
using System.Runtime.CompilerServices;
using System.Threading.Channels;
using HonestWork.Api.Shared;
using Microsoft.Extensions.Options;

namespace HonestWork.Api.Features.Observability;

public class LogStream(IOptions<ObservabilityOptions> options)
{
    private readonly ObservabilityOptions settings = options.Value;
    private readonly Queue<LogEntry> history = new();
    private readonly List<Channel<LogEntry>> followers = [];
    private readonly Lock gate = new();
    private long lastId;

    public void Write(string level, string module, string message, string? traceId, string? spanId)
    {
        lock (gate)
        {
            var entry = new LogEntry(++lastId, DateTimeOffset.UtcNow, level, module, message, traceId, spanId);

            if (history.Count == settings.LogHistorySize)
            {
                history.Dequeue();
            }

            history.Enqueue(entry);

            foreach (var follower in followers)
            {
                follower.Writer.TryWrite(entry);
            }
        }
    }

    public IReadOnlyList<LogEntry> ForTraces(IReadOnlySet<string> traceIds)
    {
        lock (gate)
        {
            return history.Where(entry => entry.TraceId is { } traceId && traceIds.Contains(traceId)).ToList();
        }
    }

    public Result<IReadOnlyList<LogEntry>> Page(long? before, int size)
    {
        if (size < 1 || size > settings.LogMaxPageSize)
        {
            return new ValidationError("size", $"Use a page size from 1 to {settings.LogMaxPageSize}.");
        }

        lock (gate)
        {
            return history.Reverse().Where(entry => before is null || entry.Id < before).Take(size).ToList();
        }
    }

    public async IAsyncEnumerable<SseItem<LogEntry?>> Follow([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var follower = Channel.CreateBounded<LogEntry>(new BoundedChannelOptions(settings.LogFollowerBacklog) { FullMode = BoundedChannelFullMode.DropOldest });

        lock (gate)
        {
            followers.Add(follower);
        }

        try
        {
            yield return new SseItem<LogEntry?>(null, "ready");

            await foreach (var entry in follower.Reader.ReadAllAsync(cancellationToken))
            {
                yield return new SseItem<LogEntry?>(entry, "log");
            }
        }
        finally
        {
            lock (gate)
            {
                followers.Remove(follower);
            }
        }
    }
}
