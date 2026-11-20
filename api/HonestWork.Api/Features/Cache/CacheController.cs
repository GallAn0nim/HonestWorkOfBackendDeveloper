using HonestWork.Api.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HonestWork.Api.Features.Cache;

[ApiController]
[Authorize]
[Route("api/cache/{sessionId:guid}")]
public class CacheController(CacheService service) : ControllerBase
{
    [HttpPost("requests")]
    public async Task<ActionResult<CacheResponse>> Request(Guid sessionId, CancellationToken cancellationToken)
    {
        var result = await service.Query(sessionId, cancellationToken);

        if (result.IsFailure)
        {
            return this.Invalid(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpPost("reset")]
    public async Task<ActionResult<CacheMetrics>> Reset(Guid sessionId)
    {
        await service.Reset(sessionId);

        return Ok(CacheMetrics.Empty);
    }
}
