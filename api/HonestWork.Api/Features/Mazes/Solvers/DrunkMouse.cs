namespace HonestWork.Api.Features.Mazes.Solvers;

public static class DrunkMouse
{
    private const long Modulus = int.MaxValue;
    private const long Multiplier = 48271;

    public static IEnumerable<MazeStep> Solve(Maze maze, int maxMoves)
    {
        var seed = 1L;
        var cell = maze.Entrance;
        var route = new List<int> { cell };
        yield return MazeStep.Visit(cell);

        for (var moves = 0; cell != maze.Exit && moves < maxMoves; moves++)
        {
            var options = maze.Neighbours(cell).ToArray();

            if (options.Length == 0)
            {
                break;
            }

            seed = seed * Multiplier % Modulus;
            cell = options[seed % options.Length];
            yield return MazeStep.Visit(cell);
            var loop = route.IndexOf(cell);

            if (loop >= 0)
            {
                route.RemoveRange(loop + 1, route.Count - loop - 1);
            }
            else
            {
                route.Add(cell);
            }
        }

        foreach (var step in route.Select(MazeStep.Path))
        {
            yield return step;
        }
    }
}
