using System.Diagnostics.Metrics;
using HonestWork.Api.Shared;
using Microsoft.Extensions.Options;

namespace HonestWork.Api.Features.Observability;

public sealed class RequestMetrics : IDisposable
{
    private const string HostingMeter = "Microsoft.AspNetCore.Hosting";
    private const string DurationInstrument = "http.server.request.duration";
    private const string OwnRoutes = "api/observability";
    private static readonly int[] Ranges = [60, 300, 900];

    private readonly ObservabilityOptions settings;
    private readonly IMeterFactory meterFactory;
    private readonly EndpointDataSource endpoints;
    private readonly MeterListener listener = new();
    private readonly Sample[] samples;
    private readonly Dictionary<EndpointKey, long> totals = [];
    private readonly Lock gate = new();
    private int next;
    private int count;
    private long requests;

    public RequestMetrics(IOptions<ObservabilityOptions> options, IMeterFactory meterFactory, EndpointDataSource endpoints)
    {
        settings = options.Value;
        samples = new Sample[settings.MetricSampleCapacity];
        this.meterFactory = meterFactory;
        this.endpoints = endpoints;
        listener.InstrumentPublished = (instrument, subscriber) =>
        {
            if (instrument.Meter.Scope == meterFactory && instrument.Meter.Name == HostingMeter && instrument.Name == DurationInstrument)
            {
                subscriber.EnableMeasurementEvents(instrument);
            }
        };
        listener.SetMeasurementEventCallback<double>(OnRequest);
        listener.Start();
    }

    public Result<MetricsSnapshot> Snapshot(int seconds)
    {
        if (!Ranges.Contains(seconds))
        {
            return new ValidationError("seconds", $"Use one of: {string.Join(", ", Ranges)}.");
        }

        var now = Environment.TickCount64;
        var reading = Read(now - seconds * 1000L);
        var window = reading.Window;
        var current = window.Count(sample => sample.At > now - settings.CurrentRpsSeconds * 1000L);
        var overview = new MetricsOverview(reading.Total, (double)current / settings.CurrentRpsSeconds, Average(window), Percentile(window, 0.95), ErrorRate(window));

        return new MetricsSnapshot(overview, RpsChart(window, now, seconds), EndpointTable(window, reading.Lifetime, now, seconds));
    }

    public void Dispose()
    {
        listener.Dispose();
    }

    private void OnRequest(Instrument instrument, double seconds, ReadOnlySpan<KeyValuePair<string, object?>> tags, object? state)
    {
        string? method = null;
        string? route = null;
        var status = 0;

        foreach (var tag in tags)
        {
            switch (tag.Key)
            {
                case "http.request.method":
                    method = tag.Value as string;
                    break;
                case "http.route":
                    route = tag.Value as string;
                    break;
                case "http.response.status_code":
                    status = Convert.ToInt32(tag.Value);
                    break;
            }
        }

        if (method is null || route is null || route.StartsWith(OwnRoutes))
        {
            return;
        }

        var milliseconds = seconds * 1000;

        lock (gate)
        {
            samples[next] = new Sample(Environment.TickCount64, method, route, status, milliseconds);
            next = (next + 1) % samples.Length;
            count = Math.Min(count + 1, samples.Length);
            var endpoint = new EndpointKey(method, route);
            totals[endpoint] = totals.GetValueOrDefault(endpoint) + 1;
            requests++;
        }
    }

    private Reading Read(long since)
    {
        var window = new List<Sample>();

        lock (gate)
        {
            for (var age = 1; age <= count; age++)
            {
                var sample = samples[(next - age + samples.Length) % samples.Length];

                if (sample.At <= since)
                {
                    break;
                }

                window.Add(sample);
            }

            return new Reading(window, new Dictionary<EndpointKey, long>(totals), requests);
        }
    }

    private double[] RpsChart(List<Sample> window, long now, int seconds)
    {
        var points = settings.ChartPoints;
        var bucketMs = seconds * 1000L / points;
        var counts = new int[points];

        foreach (var sample in window)
        {
            var bucket = points - 1 - (int)Math.Min(points - 1, (now - sample.At) / bucketMs);
            counts[bucket]++;
        }

        return counts.Select(requestsInBucket => requestsInBucket * 1000.0 / bucketMs).ToArray();
    }

    private List<EndpointMetrics> EndpointTable(List<Sample> window, Dictionary<EndpointKey, long> lifetime, long now, int seconds)
    {
        var recent = window.ToLookup(sample => new EndpointKey(sample.Method, sample.Route));

        return KnownEndpoints()
            .Union(lifetime.Keys)
            .Select(endpoint =>
            {
                var calls = recent[endpoint].ToList();

                return new EndpointMetrics(endpoint.Method, $"/{endpoint.Route}", lifetime.GetValueOrDefault(endpoint), (double)calls.Count / seconds,
                    Percentile(calls, 0.5), Percentile(calls, 0.95), ErrorRate(calls), calls.Count == 0 ? null : RpsChart(calls, now, seconds));
            })
            .OrderByDescending(endpoint => endpoint.Requests)
            .ThenBy(endpoint => endpoint.Route)
            .ToList();
    }

    private IEnumerable<EndpointKey> KnownEndpoints()
    {
        return endpoints.Endpoints
            .OfType<RouteEndpoint>()
            .Where(endpoint => endpoint.RoutePattern.RawText is { } route && route.StartsWith("api/") && !route.StartsWith(OwnRoutes))
            .SelectMany(endpoint => (endpoint.Metadata.GetMetadata<HttpMethodMetadata>()?.HttpMethods ?? [])
                .Select(method => new EndpointKey(method, endpoint.RoutePattern.RawText!)));
    }

    private static double? Average(List<Sample> calls)
    {
        return calls.Count == 0 ? null : calls.Average(sample => sample.Milliseconds);
    }

    private static double? ErrorRate(List<Sample> calls)
    {
        return calls.Count == 0 ? null : 100.0 * calls.Count(sample => sample.Status >= 500) / calls.Count;
    }

    private static double? Percentile(List<Sample> calls, double percentile)
    {
        if (calls.Count == 0)
        {
            return null;
        }

        var sorted = calls.Select(sample => sample.Milliseconds).Order().ToArray();

        return sorted[(int)Math.Ceiling(percentile * sorted.Length) - 1];
    }

    private readonly record struct Sample(long At, string Method, string Route, int Status, double Milliseconds);

    private readonly record struct EndpointKey(string Method, string Route);

    private sealed record Reading(List<Sample> Window, Dictionary<EndpointKey, long> Lifetime, long Total);
}
