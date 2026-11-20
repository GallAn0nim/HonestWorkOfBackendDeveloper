namespace HonestWork.Api.Features.Cv;

public class CvLanguage
{
    public Guid Id { get; init; }
    public Guid ProfileId { get; init; }
    public required string Name { get; init; }
    public required string Level { get; init; }
    public int SortOrder { get; init; }
}
