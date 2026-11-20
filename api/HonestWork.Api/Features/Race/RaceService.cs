using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Channels;
using HonestWork.Api.Data;
using HonestWork.Api.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace HonestWork.Api.Features.Race;

public class RaceService(IOptions<RaceOptions> options, IServiceScopeFactory scopes, ILogger<RaceService> logger)
{
    private const int Comics = 1;
    private const string UnsafeMode = "unsafe";
    private const string AtomicMode = "atomic";
    private const string QueueMode = "queue";

    private static readonly ValidationError Busy = new("race", "Too many races are running. Try again in a moment.");

    private readonly RaceOptions settings = options.Value;
    private readonly SemaphoreSlim raceLimit = new(options.Value.MaxConcurrentRaces);
    private readonly ConcurrentDictionary<Guid, PendingQueue> queues = new();

    public Task<Result<RaceResult>> RunUnsafe(CancellationToken cancellationToken)
    {
        var everyoneRead = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var reads = 0;

        return Race(async (database, comicId, customer, token) =>
        {
            var stock = await ReadStock(database, comicId, token);

            if (Interlocked.Increment(ref reads) == settings.Customers)
            {
                everyoneRead.SetResult();
            }

            await everyoneRead.Task.WaitAsync(token);

            return await BuyIfInStock(database, comicId, customer, stock, token);
        }, UnsafeMode, cancellationToken);
    }

    public Task<Result<RaceResult>> RunAtomic(CancellationToken cancellationToken)
    {
        return Race(async (database, comicId, customer, token) =>
        {
            var sold = await database.Database.ExecuteSqlAsync($"""
                WITH sold AS (
                    UPDATE "RaceComics" SET "Stock" = "Stock" - 1
                    WHERE "Id" = {comicId} AND "Stock" > 0
                    RETURNING "Id"
                )
                INSERT INTO "RaceOrders" ("ComicId", "Customer")
                SELECT "Id", {customer} FROM sold
                """, token);

            return sold == 1;
        }, AtomicMode, cancellationToken);
    }

    public async Task<Result<QueuedRace>> Enqueue(CancellationToken cancellationToken)
    {
        RemoveStaleQueues();

        if (queues.Count >= settings.MaxPendingQueues)
        {
            logger.QueueLimitReached(queues.Count);

            return Busy;
        }

        var comicId = await WithDatabase(database => CreateComic(database, cancellationToken));
        Activity.Current?.SetTag(RaceTracing.RunIdTag, comicId);
        var requests = Channel.CreateUnbounded<QueuedOrder>();
        var arrivals = Enumerable.Range(0, settings.Customers)
            .Select(customer => Task.Run(() => Arrive(requests.Writer, comicId, customer, cancellationToken)));
        await Task.WhenAll(arrivals);
        requests.Writer.Complete();
        queues[comicId] = new PendingQueue(requests.Reader, Environment.TickCount64);
        logger.OrdersQueued(requests.Reader.Count, comicId);

        return new QueuedRace(comicId, requests.Reader.Count);
    }

    public async Task<Result<RaceResult>> Process(Guid runId, CancellationToken cancellationToken)
    {
        if (!queues.TryRemove(runId, out var queue))
        {
            logger.RunMissing(runId);

            return new ValidationError("runId", "This queue was already processed or has expired. Race again.");
        }

        logger.WorkerStarted(runId, Environment.TickCount64 - queue.CreatedAt);
        Activity.Current?.SetTag(RaceTracing.RunIdTag, runId);
        var request = Activity.Current?.Context;
        using var budget = TimeBudget(cancellationToken);
        using var scope = scopes.CreateScope();
        var database = scope.ServiceProvider.GetRequiredService<HonestWorkDbContext>();
        var started = Stopwatch.GetTimestamp();
        var purchases = new bool[settings.Customers];

        try
        {
            await foreach (var order in queue.Requests.ReadAllAsync(budget.Token))
            {
                purchases[order.Customer] = await Fulfil(database, runId, order, request, budget.Token);
            }

            var duration = Stopwatch.GetElapsedTime(started).TotalMilliseconds;

            return Report(QueueMode, runId, await Summarize(database, runId, purchases, duration, budget.Token));
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            logger.TimedOut(QueueMode, settings.TimeoutSeconds);

            return Busy;
        }
    }

    private static async Task Arrive(ChannelWriter<QueuedOrder> requests, Guid runId, int customer, CancellationToken token)
    {
        using var span = RaceTracing.Source.StartActivity(ActivityKind.Producer, tags: [new(RaceTracing.RunIdTag, runId), new(RaceTracing.CustomerTag, customer)], name: RaceTracing.Enqueue);
        await requests.WriteAsync(new QueuedOrder(customer, span?.Context ?? default, Stopwatch.GetTimestamp()), token);
    }

    private async Task<bool> Fulfil(HonestWorkDbContext database, Guid runId, QueuedOrder order, ActivityContext? request, CancellationToken token)
    {
        KeyValuePair<string, object?>[] tags =
        [
            new(RaceTracing.RunIdTag, runId),
            new(RaceTracing.CustomerTag, order.Customer),
            new(RaceTracing.QueueWaitTag, Stopwatch.GetElapsedTime(order.EnqueuedAt).TotalMilliseconds),
        ];
        ActivityLink[] links = request is { } context ? [new ActivityLink(context)] : [];
        using var work = RaceTracing.Source.StartActivity(ActivityKind.Consumer, order.Trace, tags, links, name: RaceTracing.Work);

        try
        {
            var stock = await Traced(RaceTracing.ReadStock, runId, () => ReadStock(database, runId, token));
            var bought = stock > 0 && await Traced(RaceTracing.Buy, runId, () => BuyIfInStock(database, runId, order.Customer, stock, token));
            work?.SetTag(RaceTracing.OutcomeTag, bought ? RaceTracing.Purchased : RaceTracing.OutOfStock);

            if (bought)
            {
                logger.OrderPurchased(order.Customer, runId);
                return true;
            }

            logger.OrderRejected(order.Customer, runId);

            return false;
        }
        catch (Exception exception)
        {
            work?.SetStatus(ActivityStatusCode.Error, exception.GetType().Name);
            throw;
        }
    }

    private static async Task<T> Traced<T>(string name, Guid runId, Func<Task<T>> operation)
    {
        using var span = RaceTracing.Source.StartActivity(ActivityKind.Client, tags: [new(RaceTracing.RunIdTag, runId)], name: name);

        try
        {
            return await operation();
        }
        catch (Exception exception)
        {
            span?.SetStatus(ActivityStatusCode.Error, exception.GetType().Name);
            throw;
        }
    }

    private async Task<Result<RaceResult>> Race(Func<HonestWorkDbContext, Guid, int, CancellationToken, Task<bool>> buy, string mode, CancellationToken cancellationToken)
    {
        if (!await raceLimit.WaitAsync(0, cancellationToken))
        {
            logger.RaceLimitReached(mode, settings.MaxConcurrentRaces);

            return Busy;
        }

        try
        {
            using var budget = TimeBudget(cancellationToken);
            var comicId = await WithDatabase(database => CreateComic(database, budget.Token));
            var started = Stopwatch.GetTimestamp();
            var buyers = Enumerable.Range(0, settings.Customers)
                .Select(customer => Task.Run(() => WithDatabase(database => buy(database, comicId, customer, budget.Token))));
            var purchases = await Task.WhenAll(buyers);
            var duration = Stopwatch.GetElapsedTime(started).TotalMilliseconds;

            return Report(mode, comicId, await WithDatabase(database => Summarize(database, comicId, purchases, duration, budget.Token)));
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            logger.TimedOut(mode, settings.TimeoutSeconds);

            return Busy;
        }
        finally
        {
            raceLimit.Release();
        }
    }

    private RaceResult Report(string mode, Guid runId, RaceResult result)
    {
        if (result.Success != result.Orders || result.Orders + result.StockLeft != Comics)
        {
            logger.Oversold(mode, runId, result.Orders, Comics, result.DurationMs);

            return result;
        }

        logger.Consistent(mode, runId, result.Success, result.Rejected, result.DurationMs);

        return result;
    }

    private CancellationTokenSource TimeBudget(CancellationToken cancellationToken)
    {
        var budget = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        budget.CancelAfter(TimeSpan.FromSeconds(settings.TimeoutSeconds));

        return budget;
    }

    private async Task<T> WithDatabase<T>(Func<HonestWorkDbContext, Task<T>> work)
    {
        using var scope = scopes.CreateScope();

        return await work(scope.ServiceProvider.GetRequiredService<HonestWorkDbContext>());
    }

    private void RemoveStaleQueues()
    {
        var staleSince = Environment.TickCount64 - settings.PendingQueueSeconds * 1000L;

        var dropped = 0;

        foreach (var entry in queues)
        {
            if (entry.Value.CreatedAt < staleSince && queues.TryRemove(entry.Key, out _))
            {
                dropped++;
            }
        }

        if (dropped > 0)
        {
            logger.StaleQueuesDropped(dropped);
        }
    }

    private static async Task<Guid> CreateComic(HonestWorkDbContext database, CancellationToken token)
    {
        var comicId = Guid.NewGuid();
        await database.Database.ExecuteSqlAsync($"""DELETE FROM "RaceComics" WHERE "CreatedAt" < now() - interval '10 minutes'""", token);
        await database.Database.ExecuteSqlAsync($"""INSERT INTO "RaceComics" ("Id", "Stock") VALUES ({comicId}, {Comics})""", token);

        return comicId;
    }

    private static Task<int> ReadStock(HonestWorkDbContext database, Guid comicId, CancellationToken token)
    {
        return database.Database.SqlQuery<int>($"""SELECT "Stock" AS "Value" FROM "RaceComics" WHERE "Id" = {comicId}""").SingleAsync(token);
    }

    private static async Task<bool> BuyIfInStock(HonestWorkDbContext database, Guid comicId, int customer, int stock, CancellationToken token)
    {
        if (stock == 0)
        {
            return false;
        }

        await database.Database.ExecuteSqlAsync($"""UPDATE "RaceComics" SET "Stock" = {stock - 1} WHERE "Id" = {comicId}""", token);
        await database.Database.ExecuteSqlAsync($"""INSERT INTO "RaceOrders" ("ComicId", "Customer") VALUES ({comicId}, {customer})""", token);

        return true;
    }

    private static async Task<RaceResult> Summarize(HonestWorkDbContext database, Guid comicId, bool[] purchases, double duration, CancellationToken token)
    {
        var comic = await database.Database.SqlQuery<ComicState>($"""
            SELECT t."Stock", (SELECT COUNT(*)::int FROM "RaceOrders" o WHERE o."ComicId" = t."Id") AS "Orders"
            FROM "RaceComics" t
            WHERE t."Id" = {comicId}
            """).SingleAsync(token);
        var success = purchases.Count(bought => bought);

        return new RaceResult(success, purchases.Length - success, comic.Orders, comic.Stock, duration, purchases);
    }

    private record PendingQueue(ChannelReader<QueuedOrder> Requests, long CreatedAt);

    private record QueuedOrder(int Customer, ActivityContext Trace, long EnqueuedAt);
}
