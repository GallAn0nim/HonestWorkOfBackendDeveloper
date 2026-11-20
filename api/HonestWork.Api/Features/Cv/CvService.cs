using HonestWork.Api.Data;
using HonestWork.Api.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using QuestPDF.Fluent;

namespace HonestWork.Api.Features.Cv;

public class CvService(HonestWorkDbContext database, IOptions<CvOptions> options, ILogger<CvService> logger)
{
    private readonly CvOptions cv = options.Value;

    public async Task<Result<CvFile>> Render(string? language, CancellationToken cancellationToken)
    {
        var code = (language ?? cv.DefaultLanguage).ToLowerInvariant();

        if (!cv.Labels.TryGetValue(code, out var labels))
        {
            return new ValidationError("language", $"Use one of: {string.Join(", ", cv.Labels.Keys)}.");
        }

        var profile = await database.CvProfiles
            .AsSplitQuery()
            .Include(candidate => candidate.Skills.OrderBy(skill => skill.SortOrder))
            .Include(candidate => candidate.Experiences.OrderBy(experience => experience.SortOrder))
            .Include(candidate => candidate.Educations.OrderBy(education => education.SortOrder))
            .Include(candidate => candidate.Languages.OrderBy(spoken => spoken.SortOrder))
            .SingleOrDefaultAsync(candidate => candidate.LanguageCode == code, cancellationToken);

        if (profile is null)
        {
            logger.ProfileMissing(code);

            return new ValidationError("language", $"There is no CV in '{code}' yet.");
        }

        var pdf = new CvDocument(profile, labels, cv.Page).GeneratePdf();
        logger.Rendered(code, pdf.Length);

        return new CvFile(pdf, $"CV-{code}.pdf");
    }
}
