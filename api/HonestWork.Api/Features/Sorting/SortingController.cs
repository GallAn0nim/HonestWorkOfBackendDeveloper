using HonestWork.Api.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace HonestWork.Api.Features.Sorting;

[ApiController]
[Authorize]
[Route("api/sort")]
public class SortingController(SortingService service) : ControllerBase
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

    [HttpGet("random")]
    [ResponseCache(NoStore = true)]
    public ActionResult<RandomInputResponse> Random(int? count)
    {
        var result = service.GenerateRandomInput(count);

        if (result.IsFailure)
        {
            return this.Invalid(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpPost]
    [EnableRateLimiting(RateLimits.Heavy)]
    public ActionResult<SortResponse> Sort(SortRequest request)
    {
        var result = service.Sort(request);

        if (result.IsFailure)
        {
            return this.Invalid(result.Error);
        }

        return Ok(result.Value);
    }
}
