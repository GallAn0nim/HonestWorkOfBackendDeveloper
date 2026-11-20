using HonestWork.Api.Features.Mazes.Solvers;

namespace HonestWork.Api.Features.Mazes;

public static class MazeSolvers
{
    public static readonly IReadOnlyDictionary<string, Func<Maze, MazeOptions, IEnumerable<MazeStep>>> Solvers =
        new Dictionary<string, Func<Maze, MazeOptions, IEnumerable<MazeStep>>>
        {
            ["dfs"] = (maze, _) => DepthFirstSearch.Solve(maze),
            ["bfs"] = (maze, _) => BreadthFirstSearch.Solve(maze),
            ["astar"] = (maze, _) => AStarSearch.Solve(maze),
            ["dijkstra"] = (maze, _) => DijkstraSearch.Solve(maze),
            ["greedy"] = (maze, _) => GreedyBestFirstSearch.Solve(maze),
            ["bidirectional"] = (maze, _) => BidirectionalSearch.Solve(maze),
            ["tremaux"] = (maze, _) => Tremaux.Solve(maze),
            ["drunkmouse"] = (maze, limits) => DrunkMouse.Solve(maze, limits.DrunkMouseMaxMoves),
            ["wallhugger"] = (maze, limits) => WallHugger.Solve(maze, limits.WallHuggerMaxMoves),
        };
}
