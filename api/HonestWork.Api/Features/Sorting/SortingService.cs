using HonestWork.Api.Shared;
using Microsoft.Extensions.Options;

namespace HonestWork.Api.Features.Sorting;

public class SortingService(IOptions<SortingOptions> options, ILogger<SortingService> logger)
{
    private readonly SortingOptions limits = options.Value;

    public Result<AlgorithmsResponse> GetAlgorithms()
    {
        return new AlgorithmsResponse(SortingAlgorithms.Algorithms.Keys.ToList());
    }

    public Result<RandomInputResponse> GenerateRandomInput(int? count)
    {
        var itemCount = count ?? limits.DefaultCount;

        if (itemCount < 1 || itemCount > limits.MaxItems)
        {
            return new ValidationError("count", $"Use 1 to {limits.MaxItems}.");
        }

        return new RandomInputResponse(SortingAlgorithms.RandomInput(itemCount, limits.SmallestValue, limits.LargestValue, Random.Shared));
    }

    public Result<SortResponse> Sort(SortRequest request)
    {
        if (request.Algorithm is null || !SortingAlgorithms.Algorithms.TryGetValue(request.Algorithm, out var algorithm))
        {
            return new ValidationError("algorithm", $"Use one of: {string.Join(", ", SortingAlgorithms.Algorithms.Keys)}.");
        }

        if (request.Input is null || request.Input.Length > limits.MaxItems)
        {
            return new ValidationError("input", $"Send 0 to {limits.MaxItems} integers.");
        }

        var items = request.Input.Select(value => (int?)value).ToArray();
        var steps = SortRecorder.Record(items, algorithm, limits.MaxSteps);

        if (steps.Count == limits.MaxSteps)
        {
            logger.StepBudgetReached(request.Algorithm, limits.MaxSteps, items.Length);
        }
        else
        {
            logger.Sorted(request.Algorithm, items.Length, steps.Count);
        }

        return new SortResponse(request.Algorithm, steps);
    }
}
