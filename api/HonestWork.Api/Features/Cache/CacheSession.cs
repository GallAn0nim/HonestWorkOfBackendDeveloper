namespace HonestWork.Api.Features.Cache;

public class CacheSession(long now)
{
    private readonly Queue<long> arrivals = new();
    private readonly Lock gate = new();
    private long queries;
    private long requests;
    private bool cacheActive;

    public long LastSeen { get; private set; } = now;

    public void CountQuery()
    {
        Interlocked.Increment(ref queries);
    }

    public RecordedRequest Record(long now, CacheOptions options)
    {
        lock (gate)
        {
            LastSeen = now;
            arrivals.Enqueue(now);
            requests++;

            while (now - arrivals.Peek() >= 1000)
            {
                arrivals.Dequeue();
            }

            var metrics = new CacheMetrics(arrivals.Count, arrivals.Count >= options.ActivateAtRps, requests, Interlocked.Read(ref queries));
            var switched = metrics.CacheActive != cacheActive;
            cacheActive = metrics.CacheActive;

            return new RecordedRequest(metrics, switched);
        }
    }
}
