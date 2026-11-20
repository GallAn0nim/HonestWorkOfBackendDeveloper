namespace HonestWork.Api.Features.Cv;

public class CvExperience
{
    public Guid Id { get; init; }
    public Guid ProfileId { get; init; }
    public required string Company { get; init; }
    public string? CompanySummary { get; init; }
    public required string Role { get; init; }
    public DateOnly StartDate { get; init; }
    public DateOnly? EndDate { get; init; }
    public string[] Highlights { get; init; } = [];
    public string[] Technologies { get; init; } = [];
    public int SortOrder { get; init; }
}
