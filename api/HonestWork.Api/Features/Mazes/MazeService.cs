using HonestWork.Api.Shared;
using Microsoft.Extensions.Options;

namespace HonestWork.Api.Features.Mazes;

public class MazeService(IOptions<MazeOptions> options, ILogger<MazeService> logger)
{
    private const int MinRacers = 2;

    private readonly MazeOptions settings = options.Value;

    public Result<AlgorithmsResponse> GetAlgorithms()
    {
        return new AlgorithmsResponse(MazeSolvers.Solvers.Keys.ToList());
    }

    public Result<MazeGeneration> GenerateMaze(int? seed)
    {
        var generation = MazeGenerator.GenerateWithSteps(settings, seed);
        logger.Generated(generation.Maze.Seed, settings.Width, settings.Height, generation.Steps.Count);

        return generation;
    }

    public Result<MazeResponse> Solve(MazeRequest request)
    {
        if (request.Algorithm is null || !MazeSolvers.Solvers.TryGetValue(request.Algorithm, out var solve))
        {
            return new ValidationError("algorithm", $"Use one of: {string.Join(", ", MazeSolvers.Solvers.Keys)}.");
        }

        if (request.Seed is null)
        {
            return new ValidationError("seed", "Send the seed of a maze from GET /api/maze/generation.");
        }

        var steps = solve(MazeGenerator.GenerateWithSteps(settings, request.Seed).Maze, settings).ToList();
        logger.Solved(request.Algorithm, request.Seed.Value, steps.Count(step => step.Type == MazeStep.VisitType), steps.Count(step => step.Type == MazeStep.PathType));

        return new MazeResponse(request.Algorithm, steps);
    }

    public Result<MazeRaceResponse> Race(MazeRaceRequest request)
    {
        if (request.Algorithms is null || request.Algorithms.Length < MinRacers || request.Algorithms.Distinct().Count() != request.Algorithms.Length)
        {
            return new ValidationError("algorithms", $"Send at least {MinRacers} different algorithms.");
        }

        if (request.Algorithms.Any(algorithm => algorithm is null || !MazeSolvers.Solvers.ContainsKey(algorithm)))
        {
            return new ValidationError("algorithms", $"Use only: {string.Join(", ", MazeSolvers.Solvers.Keys)}.");
        }

        var maze = MazeGenerator.Generate(settings, request.Seed);
        var racers = request.Algorithms.Select(algorithm => new MazeRacer(algorithm!, MazeSolvers.Solvers[algorithm!](maze, settings).ToList())).ToList();
        var finishers = racers.Where(racer => ReachesExit(racer, maze)).ToList();
        var fewestVisits = finishers.Select(Visits).DefaultIfEmpty().Min();
        var winners = finishers.Where(racer => Visits(racer) == fewestVisits).Select(racer => racer.Algorithm).ToList();
        logger.Raced(maze.Seed, racers.Count, winners.Count > 0 ? string.Join(" and ", winners) : "nobody");

        return new MazeRaceResponse(maze, racers, winners);
    }

    private static int Visits(MazeRacer racer)
    {
        return racer.Steps.Count(step => step.Type == MazeStep.VisitType);
    }

    private static bool ReachesExit(MazeRacer racer, Maze maze)
    {
        return racer.Steps is [.., var last] && last.Type == MazeStep.PathType && last.Index == maze.Exit;
    }
}
