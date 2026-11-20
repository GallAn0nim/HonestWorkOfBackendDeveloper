namespace HonestWork.Api.Features.Mazes.Solvers;

public static class AStarSearch
{
    public static IEnumerable<MazeStep> Solve(Maze maze)
    {
        var walked = new Dictionary<int, int> { [maze.Entrance] = 0 };
        var cameFrom = new Dictionary<int, int>();
        var open = new List<int> { maze.Entrance };
        var closed = new HashSet<int>();

        int Score(int cell)
        {
            return walked[cell] + maze.DistanceToExit(cell);
        }

        while (open.Count > 0)
        {
            var best = 0;

            for (var i = 1; i < open.Count; i++)
            {
                if (Score(open[i]) < Score(open[best]))
                {
                    best = i;
                }
            }

            var cell = open[best];
            open.RemoveAt(best);
            closed.Add(cell);
            yield return MazeStep.Visit(cell);

            if (cell == maze.Exit)
            {
                foreach (var step in MazePath.To(cameFrom, cell))
                {
                    yield return step;
                }

                yield break;
            }

            foreach (var next in maze.Neighbours(cell))
            {
                if (closed.Contains(next) || walked[cell] + 1 >= walked.GetValueOrDefault(next, int.MaxValue))
                {
                    continue;
                }

                if (!walked.ContainsKey(next))
                {
                    open.Add(next);
                }

                walked[next] = walked[cell] + 1;
                cameFrom[next] = cell;
            }
        }
    }
}
