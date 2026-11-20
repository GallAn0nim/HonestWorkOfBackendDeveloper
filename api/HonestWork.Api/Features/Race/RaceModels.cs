namespace HonestWork.Api.Features.Race;

public record RaceResult(int Success, int Rejected, int Orders, int StockLeft, double DurationMs, IReadOnlyList<bool> Purchases);
public record QueuedRace(Guid RunId, int Queued);
public record ComicState(int Stock, int Orders);
