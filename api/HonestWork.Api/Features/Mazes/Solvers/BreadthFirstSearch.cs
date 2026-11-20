namespace HonestWork.Api.Features.Mazes.Solvers;

public static class BreadthFirstSearch
{
    public static IEnumerable<MazeStep> Solve(Maze maze)
    {
        var cameFrom = new Dictionary<int, int>();
        var seen = new HashSet<int> { maze.Entrance };
        var queue = new Queue<int>([maze.Entrance]);

        while (queue.Count > 0)
        {
            var cell = queue.Dequeue();
            yield return MazeStep.Visit(cell);

            if (cell == maze.Exit)
            {
                foreach (var step in MazePath.To(cameFrom, cell))
                {
                    yield return step;
                }

                yield break;
            }

            foreach (var next in maze.Neighbours(cell).Where(seen.Add))
            {
                cameFrom[next] = cell;
                queue.Enqueue(next);
            }
        }
    }
}
