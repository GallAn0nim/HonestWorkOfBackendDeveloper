namespace HonestWork.Api.Features.Race;

internal static partial class RaceLogs
{
    [LoggerMessage(EventId = 6001, Level = LogLevel.Information, Message = "Race {RaceMode} {RunId} consistent: {Sold} sold, {Rejected} rejected in {DurationMs:F1} ms")]
    public static partial void Consistent(this ILogger logger, string raceMode, Guid runId, int sold, int rejected, double durationMs);

    [LoggerMessage(EventId = 6002, Level = LogLevel.Warning, Message = "Race {RaceMode} {RunId} oversold: {Orders} orders for {Copies} copy in {DurationMs:F1} ms")]
    public static partial void Oversold(this ILogger logger, string raceMode, Guid runId, int orders, int copies, double durationMs);

    [LoggerMessage(EventId = 6003, Level = LogLevel.Warning, Message = "Race {RaceMode} rejected: {RaceLimit} races already running")]
    public static partial void RaceLimitReached(this ILogger logger, string raceMode, int raceLimit);

    [LoggerMessage(EventId = 6004, Level = LogLevel.Warning, Message = "Race {RaceMode} cancelled after its {TimeoutSeconds} s budget")]
    public static partial void TimedOut(this ILogger logger, string raceMode, int timeoutSeconds);

    [LoggerMessage(EventId = 6005, Level = LogLevel.Information, Message = "Queued {OrderCount} orders for run {RunId}")]
    public static partial void OrdersQueued(this ILogger logger, int orderCount, Guid runId);

    [LoggerMessage(EventId = 6006, Level = LogLevel.Warning, Message = "Queue rejected: {PendingQueues} queues already pending")]
    public static partial void QueueLimitReached(this ILogger logger, int pendingQueues);

    [LoggerMessage(EventId = 6007, Level = LogLevel.Information, Message = "Worker picked up run {RunId} after {QueueWaitMs} ms in the queue")]
    public static partial void WorkerStarted(this ILogger logger, Guid runId, long queueWaitMs);

    [LoggerMessage(EventId = 6008, Level = LogLevel.Warning, Message = "Run {RunId} was already processed or has expired")]
    public static partial void RunMissing(this ILogger logger, Guid runId);

    [LoggerMessage(EventId = 6009, Level = LogLevel.Information, Message = "Dropped {QueueCount} expired queues")]
    public static partial void StaleQueuesDropped(this ILogger logger, int queueCount);

    [LoggerMessage(EventId = 6010, Level = LogLevel.Information, Message = "Customer {Customer} bought the comic in run {RunId}")]
    public static partial void OrderPurchased(this ILogger logger, int customer, Guid runId);

    [LoggerMessage(EventId = 6011, Level = LogLevel.Information, Message = "Customer {Customer} rejected: out of stock in run {RunId}")]
    public static partial void OrderRejected(this ILogger logger, int customer, Guid runId);
}
