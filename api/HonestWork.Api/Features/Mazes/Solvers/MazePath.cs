namespace HonestWork.Api.Features.Mazes.Solvers;

public static class MazePath
{
    public static List<int> BackFrom(Dictionary<int, int> cameFrom, int end)
    {
        var cells = new List<int> { end };

        for (var cell = end; cameFrom.TryGetValue(cell, out var previous); cell = previous)
        {
            cells.Add(previous);
        }

        return cells;
    }

    public static IEnumerable<MazeStep> To(Dictionary<int, int> cameFrom, int goal)
    {
        return BackFrom(cameFrom, goal).AsEnumerable().Reverse().Select(MazeStep.Path);
    }
}
