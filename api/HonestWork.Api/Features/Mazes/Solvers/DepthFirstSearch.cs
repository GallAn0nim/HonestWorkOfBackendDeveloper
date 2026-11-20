namespace HonestWork.Api.Features.Mazes.Solvers;

public static class DepthFirstSearch
{
    public static IEnumerable<MazeStep> Solve(Maze maze)
    {
        var cameFrom = new Dictionary<int, int>();
        var seen = new HashSet<int>();
        var stack = new Stack<int>([maze.Entrance]);

        while (stack.Count > 0)
        {
            var cell = stack.Pop();

            if (!seen.Add(cell))
            {
                continue;
            }

            yield return MazeStep.Visit(cell);

            if (cell == maze.Exit)
            {
                foreach (var step in MazePath.To(cameFrom, cell))
                {
                    yield return step;
                }

                yield break;
            }

            foreach (var next in maze.Neighbours(cell).Reverse().Where(next => !seen.Contains(next)))
            {
                cameFrom[next] = cell;
                stack.Push(next);
            }
        }
    }
}
