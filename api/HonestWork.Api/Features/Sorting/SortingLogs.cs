namespace HonestWork.Api.Features.Sorting;

internal static partial class SortingLogs
{
    [LoggerMessage(EventId = 1001, Level = LogLevel.Information, Message = "{Algorithm} sorted {ItemCount} items in {StepCount} steps")]
    public static partial void Sorted(this ILogger logger, string algorithm, int itemCount, int stepCount);

    [LoggerMessage(EventId = 1002, Level = LogLevel.Information, Message = "{Algorithm} stopped at the {MaxSteps}-step budget with {ItemCount} items")]
    public static partial void StepBudgetReached(this ILogger logger, string algorithm, int maxSteps, int itemCount);
}
