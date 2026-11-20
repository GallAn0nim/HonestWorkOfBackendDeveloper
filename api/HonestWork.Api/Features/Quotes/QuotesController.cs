using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HonestWork.Api.Features.Quotes;

[ApiController]
[Authorize]
[Route("api/quotes")]
public class QuotesController(QuoteService service) : ControllerBase
{
    [HttpGet("random")]
    public async Task<ActionResult<Quote>> Random(int? except, CancellationToken cancellationToken)
    {
        var quote = await service.Random(except, cancellationToken);

        if (quote is null)
        {
            return NotFound();
        }

        return quote;
    }
}
