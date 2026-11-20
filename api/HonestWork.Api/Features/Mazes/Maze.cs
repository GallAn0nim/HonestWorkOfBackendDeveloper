namespace HonestWork.Api.Features.Mazes;

[Flags]
public enum Walls
{
    None = 0,
    North = 1,
    East = 2,
    South = 4,
    West = 8,
    All = North | East | South | West,
}

public record Maze(int Width, int Height, IReadOnlyList<Walls> Cells, int Entrance, int Exit, int Seed)
{
    public const int Outside = -1;

    public static readonly Walls[] Sides = [Walls.North, Walls.East, Walls.South, Walls.West];

    public static Walls Opposite(Walls side)
    {
        return side switch
        {
            Walls.North => Walls.South,
            Walls.East => Walls.West,
            Walls.South => Walls.North,
            _ => Walls.East,
        };
    }

    public int Neighbour(int cell, Walls side)
    {
        return side switch
        {
            Walls.North when cell >= Width => cell - Width,
            Walls.East when cell % Width < Width - 1 => cell + 1,
            Walls.South when cell < Width * (Height - 1) => cell + Width,
            Walls.West when cell % Width > 0 => cell - 1,
            _ => Outside,
        };
    }

    public bool HasPassage(int cell, Walls side)
    {
        return !Cells[cell].HasFlag(side) && Neighbour(cell, side) != Outside;
    }

    public IEnumerable<int> Neighbours(int cell)
    {
        return Sides.Where(side => HasPassage(cell, side)).Select(side => Neighbour(cell, side));
    }

    public int DistanceToExit(int cell)
    {
        return Math.Abs(cell % Width - Exit % Width) + Math.Abs(cell / Width - Exit / Width);
    }

    public int[] DistancesFrom(int start)
    {
        var distances = Enumerable.Repeat(-1, Cells.Count).ToArray();
        var queue = new Queue<int>([start]);
        distances[start] = 0;

        while (queue.Count > 0)
        {
            var cell = queue.Dequeue();

            foreach (var next in Neighbours(cell).Where(next => distances[next] < 0))
            {
                distances[next] = distances[cell] + 1;
                queue.Enqueue(next);
            }
        }

        return distances;
    }
}
