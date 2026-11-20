namespace HonestWork.Api.Features.Mazes;

internal static partial class MazeLogs
{
    [LoggerMessage(EventId = 2001, Level = LogLevel.Information, Message = "Maze {Seed} generated ({Width}x{Height}, {StepCount} steps)")]
    public static partial void Generated(this ILogger logger, int seed, int width, int height, int stepCount);

    [LoggerMessage(EventId = 2002, Level = LogLevel.Information, Message = "{Algorithm} explored maze {Seed}: {VisitCount} visits, {PathLength}-cell path")]
    public static partial void Solved(this ILogger logger, string algorithm, int seed, int visitCount, int pathLength);

    [LoggerMessage(EventId = 2003, Level = LogLevel.Information, Message = "Race on maze {Seed}: {RacerCount} solvers, won by {Winners}")]
    public static partial void Raced(this ILogger logger, int seed, int racerCount, string winners);
}
