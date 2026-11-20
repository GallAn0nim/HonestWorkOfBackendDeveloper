namespace HonestWork.Api.Features.Sorting;

public record SortRequest(string? Algorithm, int[]? Input);
public record SortResponse(string Algorithm, List<SortStep> Steps);
public record RandomInputResponse(int[] Input);
