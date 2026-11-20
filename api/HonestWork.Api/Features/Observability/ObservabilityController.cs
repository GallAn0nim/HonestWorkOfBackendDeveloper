using HonestWork.Api.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace HonestWork.Api.Features.Observability;

[ApiController]
[Authorize]
[Route("api/observability")]
public class ObservabilityController(LogStream logs, RequestMetrics metrics, RaceTraces traces) : ControllerBase
{
    [HttpGet("logs")]
    [EnableRateLimiting(RateLimits.LogStream)]
    public IResult Logs(CancellationToken cancellationToken)
    {
        return TypedResults.ServerSentEvents(logs.Follow(cancellationToken));
    }

    [HttpGet("logs/history")]
    [ResponseCache(NoStore = true)]
    public ActionResult<IReadOnlyList<LogEntry>> History(long? before, int size = 50)
    {
        var result = logs.Page(before, size);

        if (result.IsFailure)
        {
            return this.Invalid(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpGet("metrics")]
    [ResponseCache(NoStore = true)]
    public ActionResult<MetricsSnapshot> Metrics(int seconds = 60)
    {
        var result = metrics.Snapshot(seconds);

        if (result.IsFailure)
        {
            return this.Invalid(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpGet("traces")]
    [ResponseCache(NoStore = true)]
    public IReadOnlyList<TraceRun> Traces()
    {
        return traces.Runs();
    }

    [HttpGet("traces/{runId:guid}")]
    [ResponseCache(NoStore = true)]
    public ActionResult<TraceDetail> Trace(Guid runId)
    {
        if (traces.Find(runId) is not { } trace)
        {
            return NotFound();
        }

        return Ok(trace);
    }
}
