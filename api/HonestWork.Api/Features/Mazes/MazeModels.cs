namespace HonestWork.Api.Features.Mazes;

public record MazeRequest(string? Algorithm, int? Seed);
public record MazeResponse(string Algorithm, List<MazeStep> Steps);
public record MazeGeneration(Maze Maze, List<GenerationStep> Steps);
public record MazeRaceRequest(string?[]? Algorithms, int? Seed);
public record MazeRacer(string Algorithm, List<MazeStep> Steps);
public record MazeRaceResponse(Maze Maze, List<MazeRacer> Racers, List<string> Winners);
