namespace HonestWork.Api.Features.Sorting;

public class SortingOptions
{
    public const string Section = "Sorting";

    public int MaxItems { get; init; }
    public int SmallestValue { get; init; }
    public int LargestValue { get; init; }
    public int DefaultCount { get; init; }
    public int MaxSteps { get; init; }
}
