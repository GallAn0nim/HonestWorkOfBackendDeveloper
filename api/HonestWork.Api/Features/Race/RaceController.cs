using HonestWork.Api.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace HonestWork.Api.Features.Race;

[ApiController]
[Authorize]
[EnableRateLimiting(RateLimits.Heavy)]
[Route("api/race")]
public class RaceController(RaceService service) : ControllerBase
{
    [HttpPost("unsafe")]
    public async Task<ActionResult<RaceResult>> Unsafe(CancellationToken cancellationToken)
    {
        return Reply(await service.RunUnsafe(cancellationToken));
    }

    [HttpPost("atomic")]
    public async Task<ActionResult<RaceResult>> Atomic(CancellationToken cancellationToken)
    {
        return Reply(await service.RunAtomic(cancellationToken));
    }

    [HttpPost("queue")]
    public async Task<ActionResult<QueuedRace>> Enqueue(CancellationToken cancellationToken)
    {
        return Reply(await service.Enqueue(cancellationToken));
    }

    [HttpPost("queue/{runId:guid}/process")]
    public async Task<ActionResult<RaceResult>> Process(Guid runId, CancellationToken cancellationToken)
    {
        return Reply(await service.Process(runId, cancellationToken));
    }

    private ActionResult Reply<T>(Result<T> result)
    {
        if (result.IsFailure)
        {
            return this.Invalid(result.Error);
        }

        return Ok(result.Value);
    }
}
