namespace HonestWork.Api.Features.Cache;

public class CacheOptions
{
    public const string Section = "Cache";

    public int ActivateAtRps { get; init; } = 4;
    public int EntrySeconds { get; init; } = 30;
    public int SessionIdleSeconds { get; init; } = 120;
    public int MaxSessions { get; init; } = 200;
    public int MaxConcurrentQueries { get; init; } = 4;
    public int QueryTimeoutSeconds { get; init; } = 5;
}
