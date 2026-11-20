using HonestWork.Api.Features.Mazes;

namespace HonestWork.Api.Tests;

public class MazeGeneratorTests
{
    static readonly MazeOptions Defaults = new();

    static readonly Maze[] Mazes = Enumerable.Range(1, 20).Select(seed => MazeGenerator.Generate(Defaults, seed)).ToArray();

    static IEnumerable<int> CellsOf(Maze maze)
    {
        return Enumerable.Range(0, maze.Cells.Count);
    }

    static int Passages(Maze maze)
    {
        return CellsOf(maze).Sum(cell => maze.Neighbours(cell).Count()) / 2;
    }

    static int[] BoundaryCells(Maze maze)
    {
        return CellsOf(maze).Where(cell => Maze.Sides.Any(side => maze.Neighbour(cell, side) == Maze.Outside)).ToArray();
    }

    static int[] BoundaryOpenings(Maze maze)
    {
        return CellsOf(maze)
            .SelectMany(cell => Maze.Sides.Where(side => maze.Neighbour(cell, side) == Maze.Outside && !maze.Cells[cell].HasFlag(side)).Select(_ => cell))
            .ToArray();
    }

    [Fact]
    public void Default_maze_is_30_by_30()
    {
        Assert.All(Mazes, maze =>
        {
            Assert.Equal(30, maze.Width);
            Assert.Equal(30, maze.Height);
            Assert.Equal(900, maze.Cells.Count);
        });
    }

    [Fact]
    public void Every_cell_is_reachable()
    {
        Assert.All(Mazes, maze => Assert.DoesNotContain(-1, maze.DistancesFrom(maze.Entrance)));
    }

    [Fact]
    public void Neighbouring_cells_agree_on_their_shared_wall()
    {
        foreach (var maze in Mazes)
        {
            foreach (var cell in CellsOf(maze))
            {
                foreach (var side in Maze.Sides.Where(side => maze.Neighbour(cell, side) != Maze.Outside))
                {
                    Assert.Equal(maze.Cells[cell].HasFlag(side), maze.Cells[maze.Neighbour(cell, side)].HasFlag(Maze.Opposite(side)));
                }
            }
        }
    }

    [Fact]
    public void Only_the_entrance_and_exit_open_the_boundary()
    {
        foreach (var maze in Mazes)
        {
            Assert.NotEqual(maze.Entrance, maze.Exit);
            Assert.Equal(new[] { maze.Entrance, maze.Exit }.Order(), BoundaryOpenings(maze).Order());
        }
    }

    [Fact]
    public void Entrance_and_exit_are_the_farthest_boundary_cells()
    {
        foreach (var maze in Mazes)
        {
            var boundary = BoundaryCells(maze);
            var longest = boundary.Select(maze.DistancesFrom).Max(distances => boundary.Max(to => distances[to]));
            Assert.Equal(longest, maze.DistancesFrom(maze.Entrance)[maze.Exit]);
        }
    }

    [Fact]
    public void Without_additional_passages_the_maze_is_perfect()
    {
        var maze = MazeGenerator.Generate(new MazeOptions { AdditionalPassagesPercent = 0 }, 1);
        Assert.DoesNotContain(-1, maze.DistancesFrom(maze.Entrance));
        Assert.Equal(maze.Cells.Count - 1, Passages(maze));
    }

    [Fact]
    public void Fifteen_percent_of_the_remaining_walls_become_passages()
    {
        Assert.All(Mazes, maze => Assert.Equal(899 + 841 * 15 / 100, Passages(maze)));
    }

    [Fact]
    public void Same_seed_gives_the_same_maze()
    {
        var first = MazeGenerator.Generate(Defaults, 42);
        var second = MazeGenerator.Generate(Defaults, 42);
        Assert.Equal(first.Cells, second.Cells);
        Assert.Equal(first.Entrance, second.Entrance);
        Assert.Equal(first.Exit, second.Exit);
        Assert.Equal(42, first.Seed);
        Assert.NotEqual(first.Cells, MazeGenerator.Generate(Defaults, 43).Cells);
    }

    [Fact]
    public void Unseeded_maze_reports_a_seed_that_rebuilds_it()
    {
        var maze = MazeGenerator.Generate(Defaults);
        var rebuilt = MazeGenerator.Generate(Defaults, maze.Seed);
        Assert.Equal(maze.Cells, rebuilt.Cells);
        Assert.Equal(maze.Entrance, rebuilt.Entrance);
        Assert.Equal(maze.Exit, rebuilt.Exit);
    }

    [Fact]
    public void Replaying_the_steps_on_a_walled_grid_rebuilds_the_maze()
    {
        var generation = MazeGenerator.GenerateWithSteps(Defaults, 42);
        var maze = generation.Maze;
        var steps = generation.Steps;
        var walls = Enumerable.Repeat(Walls.All, maze.Cells.Count).ToArray();

        foreach (var step in steps)
        {
            walls[step.Cell] &= ~step.Side;
            var next = maze.Neighbour(step.Cell, step.Side);

            if (next != Maze.Outside)
            {
                walls[next] &= ~Maze.Opposite(step.Side);
            }
        }

        Assert.Equal(maze.Cells, walls);
        Assert.Equal(MazeGenerator.Generate(Defaults, 42).Cells, walls);
    }

    [Fact]
    public void Steps_carve_a_spanning_tree_then_open_passages_then_the_two_doors()
    {
        var generation = MazeGenerator.GenerateWithSteps(Defaults, 42);
        var maze = generation.Maze;
        var steps = generation.Steps;
        var reached = new HashSet<int> { steps[0].Cell };
        Assert.Equal(Enumerable.Repeat("carve", 899).Concat(Enumerable.Repeat("open", 841 * 15 / 100)).Concat(["door", "door"]), steps.Select(step => step.Type));
        Assert.All(steps.Take(899), step => Assert.True(reached.Contains(step.Cell) && reached.Add(maze.Neighbour(step.Cell, step.Side))));
        Assert.Equal([maze.Entrance, maze.Exit], steps.TakeLast(2).Select(step => step.Cell));
    }

    [Fact]
    public void Rectangular_mazes_are_connected()
    {
        var maze = MazeGenerator.Generate(new MazeOptions { Width = 12, Height = 5 }, 1);
        Assert.Equal(12, maze.Width);
        Assert.Equal(5, maze.Height);
        Assert.Equal(60, maze.Cells.Count);
        Assert.DoesNotContain(-1, maze.DistancesFrom(maze.Entrance));
    }

    [Fact]
    public void Rejects_a_grid_narrower_than_two_cells()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => MazeGenerator.Generate(new MazeOptions { Width = 1 }));
    }
}
