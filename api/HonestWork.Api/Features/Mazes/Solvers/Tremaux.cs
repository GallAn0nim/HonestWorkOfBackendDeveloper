namespace HonestWork.Api.Features.Mazes.Solvers;

public static class Tremaux
{
    public static IEnumerable<MazeStep> Solve(Maze maze)
    {
        var marked = new HashSet<int> { maze.Entrance };
        var route = new List<int> { maze.Entrance };
        yield return MazeStep.Visit(maze.Entrance);

        while (route.Count > 0 && route[^1] != maze.Exit)
        {
            var next = maze.Neighbours(route[^1]).FirstOrDefault(next => !marked.Contains(next), -1);

            if (next < 0)
            {
                route.RemoveAt(route.Count - 1);

                if (route.Count > 0)
                {
                    yield return MazeStep.Visit(route[^1]);
                }

                continue;
            }

            marked.Add(next);
            route.Add(next);
            yield return MazeStep.Visit(next);
        }

        foreach (var step in route.Select(MazeStep.Path))
        {
            yield return step;
        }
    }
}
