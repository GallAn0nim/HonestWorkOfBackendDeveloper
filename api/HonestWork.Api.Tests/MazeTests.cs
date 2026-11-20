using System.Net;
using System.Net.Http.Json;
using HonestWork.Api.Features.Mazes;
using HonestWork.Api.Shared;

namespace HonestWork.Api.Tests;

public class MazeTests
{
    static readonly string[] MayGiveUp = ["drunkmouse", "wallhugger"];

    static readonly Maze[] BraidedMazes = Enumerable.Range(1, 20).Select(seed => MazeGenerator.Generate(new MazeOptions(), seed)).ToArray();

    static readonly Maze[] PerfectMazes = Enumerable.Range(1, 20)
        .Select(seed => MazeGenerator.Generate(new MazeOptions { Width = 8, Height = 8, AdditionalPassagesPercent = 0 }, seed))
        .ToArray();

    static readonly MazeOptions Limits = new();

    static readonly Maze WalledIn = new(3, 3, Enumerable.Repeat(Walls.All, 9).ToArray(), 0, 8, 0);

    public static TheoryData<string> AllSolvers => new(MazeSolvers.Solvers.Keys);

    static int[] Steps(string solver, Maze maze, string type)
    {
        return MazeSolvers.Solvers[solver](maze, Limits).Where(step => step.Type == type).Select(step => step.Index).ToArray();
    }

    [Theory]
    [MemberData(nameof(AllSolvers))]
    public void Every_route_starts_at_the_entrance_and_follows_passages(string solver)
    {
        foreach (var maze in BraidedMazes)
        {
            var route = Steps(solver, maze, "path");

            if (route[^1] != maze.Exit)
            {
                Assert.Contains(solver, MayGiveUp);
            }

            Assert.Equal(maze.Entrance, route[0]);
            Assert.All(route.Skip(1), (cell, move) => Assert.Contains(cell, maze.Neighbours(route[move])));
        }
    }

    [Theory]
    [InlineData("bfs")]
    [InlineData("dijkstra")]
    [InlineData("astar")]
    [InlineData("bidirectional")]
    public void Shortest_path_solvers_find_a_shortest_route(string solver)
    {
        Assert.All(BraidedMazes, maze => Assert.Equal(maze.DistancesFrom(maze.Entrance)[maze.Exit] + 1, Steps(solver, maze, "path").Length));
    }

    [Theory]
    [MemberData(nameof(AllSolvers))]
    public void Every_solver_walks_the_only_route_through_a_perfect_maze(string solver)
    {
        foreach (var maze in PerfectMazes)
        {
            var route = Steps(solver, maze, "path");

            if (route[^1] != maze.Exit)
            {
                Assert.Equal("drunkmouse", solver);
                continue;
            }

            Assert.Equal(Steps("dijkstra", maze, "path"), route);
        }
    }

    [Fact]
    public void A_star_never_visits_more_than_dijkstra()
    {
        Assert.All(BraidedMazes, maze => Assert.True(Steps("astar", maze, "visit").Length <= Steps("dijkstra", maze, "visit").Length));
    }

    [Theory]
    [InlineData("drunkmouse", 9000)]
    [InlineData("wallhugger", 3600)]
    public void Walkers_stop_at_their_default_move_limit(string solver, int maxMoves)
    {
        Assert.All(BraidedMazes, maze => Assert.InRange(Steps(solver, maze, "visit").Length, 1, maxMoves + 1));
    }

    [Theory]
    [InlineData("drunkmouse")]
    [InlineData("wallhugger")]
    public void Walkers_that_run_out_of_moves_return_the_route_walked_so_far(string solver)
    {
        var fewMoves = new MazeOptions { DrunkMouseMaxMoves = 500, WallHuggerMaxMoves = 500 };
        var maze = BraidedMazes.First(candidate => MazeSolvers.Solvers[solver](candidate, fewMoves).Last().Index != candidate.Exit);
        var steps = MazeSolvers.Solvers[solver](maze, fewMoves).ToList();
        var route = steps.Where(step => step.Type == "path").Select(step => step.Index).ToArray();
        Assert.Equal(501, steps.Count(step => step.Type == "visit"));
        Assert.Equal(maze.Entrance, route[0]);
        Assert.All(route.Skip(1), (cell, move) => Assert.Contains(cell, maze.Neighbours(route[move])));
    }

    [Theory]
    [MemberData(nameof(AllSolvers))]
    public void Every_solver_stops_when_the_exit_is_unreachable(string solver)
    {
        Assert.DoesNotContain(MazeSolvers.Solvers[solver](WalledIn, Limits), step => step.Index == WalledIn.Exit);
    }
}

public class MazeEndpointTests(SignedInFactory factory) : IClassFixture<SignedInFactory>
{
    readonly HttpClient client = factory.CreateClient();

    record MazeBody(int Width, int Height, int[] Cells, int Entrance, int Exit, int Seed);

    record GenerationBody(MazeBody Maze);

    async Task<MazeBody> Maze(string query = "")
    {
        return (await client.GetFromJsonAsync<GenerationBody>($"/api/maze/generation{query}"))!.Maze;
    }

    [Fact]
    public async Task Lists_the_supported_algorithms()
    {
        var body = await client.GetFromJsonAsync<AlgorithmsResponse>("/api/maze/algorithms");
        Assert.Equal(["dfs", "bfs", "astar", "dijkstra", "greedy", "bidirectional", "tremaux", "drunkmouse", "wallhugger"], body!.Algorithms);
    }

    [Fact]
    public async Task Unseeded_maze_carries_its_seed_and_the_seed_rebuilds_it()
    {
        var maze = await Maze();
        var rebuilt = await Maze($"?seed={maze.Seed}");
        Assert.Equal(30, maze.Width);
        Assert.Equal(30, maze.Height);
        Assert.Equal(900, maze.Cells.Length);
        Assert.All(maze.Cells, walls => Assert.InRange(walls, 0, 15));
        Assert.Equal(maze.Cells, rebuilt.Cells);
        Assert.Equal(maze.Entrance, rebuilt.Entrance);
        Assert.Equal(maze.Exit, rebuilt.Exit);
    }

    [Fact]
    public async Task Generation_serves_the_maze_with_its_steps_in_the_page_format()
    {
        var body = await client.GetStringAsync("/api/maze/generation?seed=7");
        Assert.StartsWith("""{"maze":{"width":30,"height":30,"cells":[""", body);
        Assert.Contains("""],"entrance":""", body);
        Assert.Contains(""","seed":7},"steps":[{"type":"carve","cell":""", body);
        Assert.Matches("""\{"type":"door","cell":\d+,"side":(1|2|4|8)\}\]\}$""", body);
    }

    [Fact]
    public async Task Solves_the_maze_of_the_given_seed_in_the_page_format()
    {
        var maze = await Maze("?seed=7");
        var response = await client.PostAsJsonAsync("/api/maze", new { algorithm = "dijkstra", seed = 7 });
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadAsStringAsync();
        Assert.StartsWith($$"""{"algorithm":"dijkstra","steps":[{"type":"visit","index":{{maze.Entrance}}}""", body);
        Assert.EndsWith($$"""{"type":"path","index":{{maze.Exit}}}]}""", body);
    }

    [Fact]
    public async Task Races_the_solvers_on_one_maze_and_names_the_fewest_visits_to_the_exit_as_winner()
    {
        var maze = await Maze("?seed=7");
        var response = await client.PostAsJsonAsync("/api/maze/race", new { algorithms = new[] { "dfs", "bfs", "astar", "drunkmouse", "wallhugger" }, seed = 7 });
        response.EnsureSuccessStatusCode();
        var body = (await response.Content.ReadFromJsonAsync<RaceBody>())!;

        Assert.Equal(maze.Cells, body.Maze.Cells);
        Assert.Equal(["dfs", "bfs", "astar", "drunkmouse", "wallhugger"], body.Racers.Select(racer => racer.Algorithm));

        var finishers = body.Racers.Where(racer => racer.Steps[^1] is { Type: "path" } last && last.Index == maze.Exit).ToList();
        var fewestVisits = finishers.Min(Visits);
        Assert.Equal(finishers.Where(racer => Visits(racer) == fewestVisits).Select(racer => racer.Algorithm), body.Winners);
        Assert.DoesNotContain("drunkmouse", body.Winners);
    }

    [Theory]
    [InlineData("""{"seed":7}""")]
    [InlineData("""{"algorithms":["astar"],"seed":7}""")]
    [InlineData("""{"algorithms":["astar","astar"],"seed":7}""")]
    [InlineData("""{"algorithms":["astar","teleport"],"seed":7}""")]
    [InlineData("""{"algorithms":["astar",null],"seed":7}""")]
    public async Task Rejects_bad_races(string body)
    {
        var response = await client.PostAsync("/api/maze/race", new StringContent(body, System.Text.Encoding.UTF8, "application/json"));
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    record StepBody(string Type, int Index);

    record RacerBody(string Algorithm, StepBody[] Steps);

    record RaceBody(MazeBody Maze, RacerBody[] Racers, string[] Winners);

    static int Visits(RacerBody racer)
    {
        return racer.Steps.Count(step => step.Type == "visit");
    }

    [Theory]
    [InlineData("""{"algorithm":"teleport","seed":7}""")]
    [InlineData("""{"algorithm":"astar"}""")]
    [InlineData("""{"seed":7}""")]
    [InlineData("""{"algorithm":"astar","seed":"seven"}""")]
    public async Task Rejects_bad_requests(string body)
    {
        var response = await client.PostAsync("/api/maze", new StringContent(body, System.Text.Encoding.UTF8, "application/json"));
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
