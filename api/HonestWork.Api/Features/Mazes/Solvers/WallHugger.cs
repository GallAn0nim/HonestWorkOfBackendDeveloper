namespace HonestWork.Api.Features.Mazes.Solvers;

public static class WallHugger
{
    private static readonly int[] TurnsRightFirst = [1, 0, 3, 2];

    public static IEnumerable<MazeStep> Solve(Maze maze, int maxMoves)
    {
        var cell = maze.Entrance;
        var facing = 1;
        var route = new List<int> { cell };
        yield return MazeStep.Visit(cell);

        for (var moves = 0; cell != maze.Exit && moves < maxMoves; moves++)
        {
            var turn = Array.FindIndex(TurnsRightFirst, offset => maze.HasPassage(cell, Maze.Sides[(facing + offset) % 4]));

            if (turn < 0)
            {
                break;
            }

            facing = (facing + TurnsRightFirst[turn]) % 4;
            cell = maze.Neighbour(cell, Maze.Sides[facing]);
            yield return MazeStep.Visit(cell);

            if (route.Count > 1 && route[^2] == cell)
            {
                route.RemoveAt(route.Count - 1);
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
