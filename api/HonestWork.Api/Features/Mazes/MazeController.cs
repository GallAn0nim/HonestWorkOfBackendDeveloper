using HonestWork.Api.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace HonestWork.Api.Features.Mazes;

[ApiController]
[Authorize]
[Route("api/maze")]
public class MazeController(MazeService service) : ControllerBase
{
    [HttpGet("algorithms")]
    public ActionResult<AlgorithmsResponse> Algorithms()
    {
        var result = service.GetAlgorithms();

        if (result.IsFailure)
        {
            return this.Invalid(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpGet("generation")]
    [ResponseCache(NoStore = true)]
    public ActionResult<MazeGeneration> Generation(int? seed)
    {
        var result = service.GenerateMaze(seed);

        if (result.IsFailure)
        {
            return this.Invalid(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpPost]
    [EnableRateLimiting(RateLimits.Heavy)]
    public ActionResult<MazeResponse> Solve(MazeRequest request)
    {
        var result = service.Solve(request);

        if (result.IsFailure)
        {
            return this.Invalid(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpPost("race")]
    [EnableRateLimiting(RateLimits.Heavy)]
    public ActionResult<MazeRaceResponse> Race(MazeRaceRequest request)
    {
        var result = service.Race(request);

        if (result.IsFailure)
        {
            return this.Invalid(result.Error);
        }

        return Ok(result.Value);
    }
}
