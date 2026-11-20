namespace HonestWork.Api.Features.Mazes;

public static class MazeGenerator
{
    private const int MinSize = 2;

    private static readonly Walls[] ForwardSides = [Walls.East, Walls.South];

    public static Maze Generate(MazeOptions options, int? seed = null)
    {
        return GenerateWithSteps(options, seed).Maze;
    }

    public static MazeGeneration GenerateWithSteps(MazeOptions options, int? seed = null)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(options.Width, MinSize);
        ArgumentOutOfRangeException.ThrowIfLessThan(options.Height, MinSize);

        var usedSeed = seed ?? Random.Shared.Next();
        var random = new Random(usedSeed);
        var walls = Enumerable.Repeat(Walls.All, options.Width * options.Height).ToArray();
        var maze = new Maze(options.Width, options.Height, Array.AsReadOnly(walls), 0, 0, usedSeed);

        var steps = new List<GenerationStep>();

        CarvePerfectMaze(maze, walls, random, steps);
        OpenAdditionalPassages(maze, walls, random, options.AdditionalPassagesPercent, steps);

        var withDoors = PlaceDoorsFarthestApart(maze);

        foreach (var door in new[] { withDoors.Entrance, withDoors.Exit })
        {
            var side = BoundarySide(maze, door);
            walls[door] &= ~side;
            steps.Add(GenerationStep.Door(door, side));
        }

        return new MazeGeneration(withDoors, steps);
    }

    private static void CarvePerfectMaze(Maze maze, Walls[] walls, Random random, List<GenerationStep> steps)
    {
        var visited = new bool[walls.Length];
        var frontier = new List<WallSide>();
        Visit(random.Next(walls.Length));

        while (frontier.Count > 0)
        {
            var pick = random.Next(frontier.Count);
            var wall = frontier[pick];
            frontier[pick] = frontier[^1];
            frontier.RemoveAt(frontier.Count - 1);
            var next = maze.Neighbour(wall.Cell, wall.Side);

            if (visited[next])
            {
                continue;
            }

            RemoveWall(maze, walls, wall.Cell, wall.Side);
            steps.Add(GenerationStep.Carve(wall.Cell, wall.Side));
            Visit(next);
        }

        void Visit(int cell)
        {
            visited[cell] = true;
            frontier.AddRange(Maze.Sides.Where(side => maze.Neighbour(cell, side) != Maze.Outside).Select(side => new WallSide(cell, side)));
        }
    }

    private static void OpenAdditionalPassages(Maze maze, Walls[] walls, Random random, int percent, List<GenerationStep> steps)
    {
        var closed = Enumerable.Range(0, walls.Length)
            .SelectMany(cell => ForwardSides
                .Where(side => walls[cell].HasFlag(side) && maze.Neighbour(cell, side) != Maze.Outside)
                .Select(side => new WallSide(cell, side)))
            .ToArray();
        random.Shuffle(closed);

        foreach (var wall in closed.Take(closed.Length * percent / 100))
        {
            RemoveWall(maze, walls, wall.Cell, wall.Side);
            steps.Add(GenerationStep.Open(wall.Cell, wall.Side));
        }
    }

    private static Maze PlaceDoorsFarthestApart(Maze maze)
    {
        var boundary = Enumerable.Range(0, maze.Cells.Count).Where(cell => BoundarySide(maze, cell) != Walls.None).ToArray();
        var entrance = boundary[0];
        var exit = boundary[0];
        var longest = 0;

        foreach (var from in boundary)
        {
            var distances = maze.DistancesFrom(from);
            var farthest = boundary.MaxBy(cell => distances[cell]);

            if (distances[farthest] > longest)
            {
                entrance = from;
                exit = farthest;
                longest = distances[farthest];
            }
        }

        return maze with { Entrance = entrance, Exit = exit };
    }

    private static Walls BoundarySide(Maze maze, int cell)
    {
        return Maze.Sides.FirstOrDefault(side => maze.Neighbour(cell, side) == Maze.Outside);
    }

    private static void RemoveWall(Maze maze, Walls[] walls, int cell, Walls side)
    {
        walls[cell] &= ~side;
        walls[maze.Neighbour(cell, side)] &= ~Maze.Opposite(side);
    }

    private readonly record struct WallSide(int Cell, Walls Side);
}
