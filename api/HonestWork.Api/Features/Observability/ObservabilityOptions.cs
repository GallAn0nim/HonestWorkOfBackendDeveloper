namespace HonestWork.Api.Features.Observability;

public class ObservabilityOptions
{
    public const string Section = "Observability";

    public int LogHistorySize { get; init; } = 500;
    public int LogFollowerBacklog { get; init; } = 250;
    public int LogMaxPageSize { get; init; } = 100;
    public int MetricSampleCapacity { get; init; } = 50_000;
    public int CurrentRpsSeconds { get; init; } = 5;
    public int ChartPoints { get; init; } = 60;
    public int TraceRunCapacity { get; init; } = 10;
}
