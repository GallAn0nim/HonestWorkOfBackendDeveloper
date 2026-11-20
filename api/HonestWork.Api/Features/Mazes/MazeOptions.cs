namespace HonestWork.Api.Features.Mazes;

public class MazeOptions
{
    public const string Section = "Maze";

    public int Width { get; init; } = 30;
    public int Height { get; init; } = 30;
    public int AdditionalPassagesPercent { get; init; } = 15;
    public int DrunkMouseMaxMoves { get; init; } = 9000;
    public int WallHuggerMaxMoves { get; init; } = 3600;
}
