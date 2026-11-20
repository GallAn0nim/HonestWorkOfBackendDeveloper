namespace HonestWork.Api.Features.Race;

public class RaceOptions
{
    public const string Section = "Race";

    public int Customers { get; init; } = 20;
    public int MaxConcurrentRaces { get; init; } = 2;
    public int MaxPendingQueues { get; init; } = 50;
    public int PendingQueueSeconds { get; init; } = 60;
    public int TimeoutSeconds { get; init; } = 5;
}
