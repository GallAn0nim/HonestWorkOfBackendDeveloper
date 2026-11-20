namespace HonestWork.Api.Features.Cv;

public class CvEducation
{
    public Guid Id { get; init; }
    public Guid ProfileId { get; init; }
    public required string Institution { get; init; }
    public required string Degree { get; init; }
    public string? Field { get; init; }
    public string? Thesis { get; init; }
    public DateOnly StartDate { get; init; }
    public DateOnly? EndDate { get; init; }
    public int SortOrder { get; init; }
}
