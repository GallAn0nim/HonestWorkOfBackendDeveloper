using System.Net;
using System.Net.Http.Json;
using HonestWork.Api.Features.Sorting;
using HonestWork.Api.Shared;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace HonestWork.Api.Tests;

public class SortingTests
{
    const int NoLimit = int.MaxValue;

    record SortRun(int?[] Items, List<SortStep> Steps);

    static SortRun Run(string algorithm, int[] input, int maxSteps = NoLimit)
    {
        var items = input.Select(value => (int?)value).ToArray();
        var steps = SortRecorder.Record(items, SortingAlgorithms.Algorithms[algorithm], maxSteps);
        return new SortRun(items, steps);
    }

    static IEnumerable<int[]> Inputs()
    {
        var random = new Random(42);
        yield return [];
        yield return [42];
        yield return [5, 5, 5];
        yield return [9, 8, 7, 6, 5, 4, 3, 2, 1];

        for (var sample = 0; sample < 200; sample++)
        {
            yield return Enumerable.Range(0, random.Next(20)).Select(_ => random.Next(10, 90)).ToArray();
        }
    }

    [Theory]
    [InlineData("bubble")]
    [InlineData("selection")]
    [InlineData("insertion")]
    [InlineData("quick")]
    [InlineData("merge")]
    [InlineData("heap")]
    [InlineData("shell")]
    [InlineData("cycle")]
    public void Gives_a_sorted_permutation(string algorithm)
    {
        foreach (var input in Inputs())
        {
            Assert.Equal(input.Order().Select(value => (int?)value), Run(algorithm, input).Items);
        }
    }

    [Theory]
    [InlineData("bogo")]
    [InlineData("bozo")]
    public void Random_sorts_finish_small_inputs(string algorithm)
    {
        Assert.Equal(new int?[] { 1, 2, 3, 4 }, Run(algorithm, [3, 1, 4, 2]).Items);
    }

    [Theory]
    [InlineData("bogo")]
    [InlineData("bozo")]
    public void Random_sorts_stop_at_the_step_limit(string algorithm)
    {
        var input = Enumerable.Range(0, 32).Reverse().ToArray();
        Assert.Equal(5_000, Run(algorithm, input, 5_000).Steps.Count);
    }

    [Theory]
    [InlineData("bubble")]
    [InlineData("selection")]
    [InlineData("insertion")]
    [InlineData("quick")]
    [InlineData("merge")]
    [InlineData("heap")]
    [InlineData("shell")]
    [InlineData("cycle")]
    [InlineData("purge")]
    public void Finishes_the_largest_input_within_the_step_limit(string algorithm)
    {
        int[][] worstCases =
        [
            Enumerable.Range(0, 32).Reverse().ToArray(),
            Enumerable.Range(0, 32).ToArray(),
            Enumerable.Repeat(50, 32).ToArray(),
            Enumerable.Range(0, 32).Select(index => index % 2 == 0 ? index : 32 - index).ToArray(),
        ];

        foreach (var input in worstCases)
        {
            var steps = Run(algorithm, input).Steps.Count;
            Assert.True(steps < 5_000, $"{algorithm} took {steps} steps");
        }
    }

    [Fact]
    public void Cycle_sort_writes_each_value_at_most_once()
    {
        var input = new[] { 50, 20, 90, 20, 10, 70, 30 };
        Assert.True(Run("cycle", input).Steps.Count(step => step.Type == "write") <= input.Length);
    }

    [Fact]
    public void Purge_keeps_order_but_loses_values()
    {
        Assert.Equal(new int?[] { 30, null, 50, null, 60 }, Run("purge", [30, 10, 50, 20, 60]).Items);
    }

    [Fact]
    public void Bubble_reports_every_step()
    {
        Assert.Equal([SortStep.Compare(0, 1), SortStep.Write(0, 1), SortStep.Write(1, 2)], Run("bubble", [2, 1]).Steps);
    }
}

public class SortEndpointTests(SignedInFactory factory) : IClassFixture<SignedInFactory>
{
    readonly HttpClient client = factory.CreateClient();
    readonly int maxItems = factory.Services.GetRequiredService<IOptions<SortingOptions>>().Value.MaxItems;

    [Fact]
    public async Task Lists_the_supported_algorithms()
    {
        var body = await client.GetFromJsonAsync<AlgorithmsResponse>("/api/sort/algorithms");
        Assert.Equal(["bubble", "selection", "insertion", "quick", "merge", "heap", "shell", "cycle", "bogo", "bozo", "purge"], body!.Algorithms);
    }

    [Fact]
    public async Task Returns_operations_in_the_page_format()
    {
        var response = await client.PostAsJsonAsync("/api/sort", new { algorithm = "bubble", input = new[] { 2, 1 } });
        response.EnsureSuccessStatusCode();
        Assert.Equal(
            """{"algorithm":"bubble","steps":[{"type":"compare","index":0,"otherIndex":1},{"type":"write","index":0,"value":1},{"type":"write","index":1,"value":2}]}""",
            await response.Content.ReadAsStringAsync());
    }

    [Theory]
    [InlineData("""{"algorithm":"spaghetti","input":[1]}""")]
    [InlineData("""{"input":[1]}""")]
    [InlineData("""{"algorithm":"merge"}""")]
    public async Task Rejects_bad_requests(string body)
    {
        var response = await client.PostAsync("/api/sort", new StringContent(body, System.Text.Encoding.UTF8, "application/json"));
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Rejects_too_many_items()
    {
        var response = await client.PostAsJsonAsync("/api/sort", new { algorithm = "merge", input = new int[maxItems + 1] });
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
