using HonestWork.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace HonestWork.Api.Features.Quotes;

public class QuoteService(HonestWorkDbContext database)
{
    public Task<Quote?> Random(int? except, CancellationToken cancellationToken)
    {
        return database.Quotes
            .AsNoTracking()
            .OrderBy(quote => quote.Id == except)
            .ThenBy(_ => EF.Functions.Random())
            .FirstOrDefaultAsync(cancellationToken);
    }
}
