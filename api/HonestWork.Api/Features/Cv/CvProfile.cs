namespace HonestWork.Api.Features.Cv;

public class CvProfile
{
    public Guid Id { get; init; }
    public required string LanguageCode { get; init; }
    public required string FullName { get; init; }
    public required string Title { get; init; }
    public required string Email { get; init; }
    public required string WebsiteUrl { get; init; }
    public required string WorkPreference { get; init; }
    public List<CvSkill> Skills { get; init; } = [];
    public List<CvExperience> Experiences { get; init; } = [];
    public List<CvEducation> Educations { get; init; } = [];
    public List<CvLanguage> Languages { get; init; } = [];
}
