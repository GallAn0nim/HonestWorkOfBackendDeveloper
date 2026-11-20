using System.Diagnostics;

namespace HonestWork.Api.Features.Race;

public static class RaceTracing
{
    public const string SourceName = "HonestWork.Race";
    public const string RunIdTag = "race.run_id";
    public const string CustomerTag = "race.customer";
    public const string QueueWaitTag = "race.queue_wait_ms";
    public const string OutcomeTag = "race.outcome";
    public const string Enqueue = "queue.enqueue";
    public const string Work = "worker.process";
    public const string ReadStock = "db.read_stock";
    public const string Buy = "db.buy";
    public const string Purchased = "purchased";
    public const string OutOfStock = "out_of_stock";

    public static readonly ActivitySource Source = new(SourceName);
}
