namespace HonestWork.Api.Features.Cv;

public class CvSkill
{
    public Guid Id { get; init; }
    public Guid ProfileId { get; init; }
    public required string Category { get; init; }
    public required string Name { get; init; }
    public int SortOrder { get; init; }
}
