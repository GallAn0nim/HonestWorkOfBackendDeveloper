using HonestWork.Api.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Net.Http.Headers;

namespace HonestWork.Api.Features.Cv;

[ApiController]
[Route("api/cv")]
public class CvController(CvService service) : ControllerBase
{
    [HttpGet]
    [EnableRateLimiting(RateLimits.Heavy)]
    public async Task<ActionResult> Get(string? language, CancellationToken cancellationToken)
    {
        var result = await service.Render(language, cancellationToken);

        if (result.IsFailure)
        {
            return this.Invalid(result.Error);
        }

        Response.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = result.Value.FileName }.ToString();

        return File(result.Value.Content, "application/pdf");
    }
}
