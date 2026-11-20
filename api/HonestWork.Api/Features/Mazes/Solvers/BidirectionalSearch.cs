namespace HonestWork.Api.Features.Mazes.Solvers;

public static class BidirectionalSearch
{
    public static IEnumerable<MazeStep> Solve(Maze maze)
    {
        var queues = new[] { new Queue<int>([maze.Entrance]), new Queue<int>([maze.Exit]) };
        var cameFrom = new[] { new Dictionary<int, int>(), new Dictionary<int, int>() };
        var seen = new[] { new HashSet<int> { maze.Entrance }, new HashSet<int> { maze.Exit } };

        for (var side = 0; queues[0].Count > 0 && queues[1].Count > 0; side = 1 - side)
        {
            var cell = queues[side].Dequeue();
            yield return MazeStep.Visit(cell);

            if (seen[1 - side].Contains(cell))
            {
                var fromStart = MazePath.BackFrom(cameFrom[0], cell).AsEnumerable().Reverse();
                var toGoal = MazePath.BackFrom(cameFrom[1], cell).Skip(1);

                foreach (var step in fromStart.Concat(toGoal).Select(MazeStep.Path))
                {
                    yield return step;
                }

                yield break;
            }

            foreach (var next in maze.Neighbours(cell).Where(seen[side].Add))
            {
                cameFrom[side][next] = cell;
                queues[side].Enqueue(next);
            }
        }
    }
}
