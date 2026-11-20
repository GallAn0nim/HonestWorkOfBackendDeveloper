using System.Diagnostics;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using HonestWork.Api.Features.Observability;
using HonestWork.Api.Features.Race;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HonestWork.Api.Tests;

public class ObservabilityTests(SignedInFactory factory) : IClassFixture<SignedInFactory>
{
    static readonly TimeSpan Patience = TimeSpan.FromSeconds(5);
    static readonly IOptions<ObservabilityOptions> Settings = Options.Create(new ObservabilityOptions());

    readonly HttpClient client = factory.CreateClient();

    [Fact]
    public async Task Metrics_count_real_requests_per_route()
    {
        await client.GetAsync("/api/sort/random?count=3");
        await client.GetAsync("/api/sort/random?count=4");

        var endpoint = await WaitFor(async () =>
        {
            var snapshot = await client.GetFromJsonAsync<MetricsSnapshot>("/api/observability/metrics");
            var random = snapshot!.Endpoints.Single(row => row is { Method: "GET", Route: "/api/sort/random" });

            return random.Requests >= 2 ? random : null;
        });

        Assert.Equal(0, endpoint.ErrorRate);
        Assert.NotNull(endpoint.P95Ms);
        Assert.True(endpoint.RpsSeries!.Sum() > 0);
    }

    [Fact]
    public async Task Metrics_list_endpoints_that_were_never_called()
    {
        var snapshot = await client.GetFromJsonAsync<MetricsSnapshot>("/api/observability/metrics?seconds=900");

        Assert.Contains(snapshot!.Endpoints, row => row is { Method: "POST", Route: "/api/race/atomic", RpsSeries: null });
        Assert.DoesNotContain(snapshot.Endpoints, row => row.Route.StartsWith("/api/observability"));
        Assert.Equal(60, snapshot.Rps.Length);
    }

    [Fact]
    public async Task Metrics_reject_unknown_ranges()
    {
        var response = await client.GetAsync("/api/observability/metrics?seconds=7");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Log_stream_follows_new_module_logs()
    {
        using var timeout = new CancellationTokenSource(Patience);
        using var response = await client.GetAsync("/api/observability/logs", HttpCompletionOption.ResponseHeadersRead, timeout.Token);
        using var reader = new StreamReader(await response.Content.ReadAsStreamAsync(timeout.Token));

        Assert.Equal("text/event-stream", response.Content.Headers.ContentType?.MediaType);

        await client.PostAsJsonAsync("/api/sort", new { algorithm = "bubble", input = new[] { 3, 1, 2 } });

        while (await reader.ReadLineAsync(timeout.Token) is { } line)
        {
            if (line.Contains("\"module\":\"Sorting\"") && line.Contains("sorted 3 items"))
            {
                return;
            }
        }

        Assert.Fail("The sort log never arrived.");
    }

    [Fact]
    public void Log_history_pages_from_newest_to_oldest()
    {
        var stream = new LogStream(Settings);

        for (var index = 1; index <= 5; index++)
        {
            stream.Write("INFO", "Sorting", $"entry {index}", null, null);
        }

        var first = stream.Page(null, 2).Value.Select(entry => entry.Id);
        var second = stream.Page(4, 2).Value.Select(entry => entry.Id);
        var last = stream.Page(2, 2).Value.Select(entry => entry.Id);

        Assert.Equal([5L, 4L], first);
        Assert.Equal([3L, 2L], second);
        Assert.Equal([1L], last);
    }

    [Fact]
    public async Task Log_history_rejects_oversized_pages()
    {
        var response = await client.GetAsync("/api/observability/logs/history?size=1000");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public void Log_stream_keeps_exception_details_out()
    {
        var stream = new LogStream(Settings);
        var logger = new LogStreamLoggerProvider(stream).CreateLogger("HonestWork.Api.Features.Race.RaceService");

        logger.LogError(new InvalidOperationException("Password=hunter2"), "Race {RaceMode} failed", "atomic");

        var entry = Assert.Single(stream.Page(null, 10).Value);
        Assert.Equal("ERROR", entry.Level);
        Assert.Equal("Race", entry.Module);
        Assert.Equal("Race atomic failed (InvalidOperationException)", entry.Message);
    }

    [Fact]
    public async Task Log_stream_keeps_visitor_details_out()
    {
        const string typedUsername = "visitor-typed-name";
        const string userAgent = "VisitorBrowser/1.0";
        var address = IPAddress.Parse("203.0.113.7");

        await SendAsVisitor("/api/auth/login", new { username = typedUsername, password = "wrong" }, userAgent, address);
        await SendAsVisitor("/api/auth/recover", new { username = typedUsername }, userAgent, address);

        var messages = factory.Services.GetRequiredService<LogStream>().Page(null, 100).Value.Select(entry => entry.Message).ToList();

        Assert.Contains(messages, message => message.StartsWith("Sign-in failed"));
        Assert.Contains(messages, message => message.StartsWith("Password recovery matched no user"));
        Assert.DoesNotContain(messages, message => message.Contains(typedUsername) || message.Contains(userAgent) || message.Contains(address.ToString()));
    }

    [Fact]
    public async Task Queue_race_traces_every_order_across_the_channel()
    {
        var queued = await (await client.PostAsync("/api/race/queue", null)).Content.ReadFromJsonAsync<QueuedRace>();
        var raced = await (await client.PostAsync($"/api/race/queue/{queued!.RunId}/process", null)).Content.ReadFromJsonAsync<RaceResult>();

        var trace = await WaitFor(async () =>
        {
            var detail = await client.GetFromJsonAsync<TraceDetail>($"/api/observability/traces/{queued.RunId}");

            return detail!.Spans.Count(span => span.Source == "API" && span.DurationMs is not null) == 2 ? detail : null;
        });

        var enqueueRequest = trace.Spans.Single(span => span.Name.EndsWith("api/race/queue"));
        var processRequest = trace.Spans.Single(span => span.Name.EndsWith("/process"));
        var enqueues = trace.Spans.Where(span => span.Name == RaceTracing.Enqueue).ToList();
        var workers = trace.Spans.Where(span => span.Name == RaceTracing.Work).ToList();

        Assert.Equal(1, raced!.Success);
        Assert.Equal(19, raced.Rejected);
        Assert.Equal(20, enqueues.Count);
        Assert.All(enqueues, span => Assert.Equal(enqueueRequest.SpanId, span.ParentSpanId));
        Assert.Equal(20, workers.Count);
        Assert.All(workers, worker =>
        {
            var enqueue = enqueues.Single(span => span.Customer == worker.Customer);
            Assert.Equal(enqueue.TraceId, worker.TraceId);
            Assert.Equal(enqueue.SpanId, worker.ParentSpanId);
            Assert.Equal(processRequest.SpanId, worker.LinkedSpanId);
            Assert.True(worker.QueueWaitMs > 0);
            Assert.False(worker.Failed);
        });
        Assert.Equal(1, workers.Count(span => span.Outcome == RaceTracing.Purchased));
        Assert.Equal(19, workers.Count(span => span.Outcome == RaceTracing.OutOfStock));
        Assert.Equal(20, trace.Spans.Count(span => span.Name == RaceTracing.ReadStock));
        Assert.Single(trace.Spans, span => span.Name == RaceTracing.Buy);
        Assert.Equal(20, trace.Logs.Count(entry => entry.Message.StartsWith("Customer ") && workers.Any(worker => worker.SpanId == entry.SpanId)));
        var runs = await client.GetFromJsonAsync<List<TraceRun>>("/api/observability/traces");
        Assert.Contains(runs!, run => run is { Requests: 20, Finished: 20 } && run.RunId == queued.RunId);
    }

    [Fact]
    public void Race_traces_keep_only_recent_runs()
    {
        using var traces = new RaceTraces(Settings, new LogStream(Settings));
        var runIds = Enumerable.Range(0, 11).Select(_ => Guid.NewGuid()).ToList();

        foreach (var runId in runIds)
        {
            RaceTracing.Source.StartActivity(ActivityKind.Producer, tags: [new(RaceTracing.RunIdTag, runId)], name: RaceTracing.Enqueue)!.Stop();
        }

        Assert.Equal(10, traces.Runs().Count);
        Assert.Null(traces.Find(runIds[0]));
        Assert.NotNull(traces.Find(runIds[^1]));
    }

    Task<HttpContext> SendAsVisitor(string path, object body, string userAgent, IPAddress address)
    {
        return factory.Server.SendAsync(context =>
        {
            context.Request.Method = HttpMethods.Post;
            context.Request.Path = path;
            context.Request.ContentType = "application/json";
            context.Request.Headers.UserAgent = userAgent;
            context.Request.Body = new MemoryStream(JsonSerializer.SerializeToUtf8Bytes(body));
            context.Connection.RemoteIpAddress = address;
        });
    }

    static async Task<T> WaitFor<T>(Func<Task<T?>> probe) where T : class
    {
        using var timeout = new CancellationTokenSource(Patience);

        while (!timeout.IsCancellationRequested)
        {
            if (await probe() is { } value)
            {
                return value;
            }

            await Task.Delay(50);
        }

        throw new TimeoutException();
    }
}
