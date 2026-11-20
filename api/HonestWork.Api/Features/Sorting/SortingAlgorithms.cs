using HonestWork.Api.Features.Sorting.Algorithms;

namespace HonestWork.Api.Features.Sorting;

public static class SortingAlgorithms
{
    public static int[] RandomInput(int count, int smallestValue, int largestValue, Random random)
    {
        return Enumerable.Range(0, count).Select(_ => random.Next(smallestValue, largestValue + 1)).ToArray();
    }

    public static readonly IReadOnlyDictionary<string, Action<SortRecorder>> Algorithms =
        new Dictionary<string, Action<SortRecorder>>
        {
            ["bubble"] = BubbleSort.Sort,
            ["selection"] = SelectionSort.Sort,
            ["insertion"] = InsertionSort.Sort,
            ["quick"] = QuickSort.Sort,
            ["merge"] = MergeSort.Sort,
            ["heap"] = HeapSort.Sort,
            ["shell"] = ShellSort.Sort,
            ["cycle"] = CycleSort.Sort,
            ["bogo"] = items => BogoSort.Sort(items, Random.Shared),
            ["bozo"] = items => BozoSort.Sort(items, Random.Shared),
            ["purge"] = PurgeSort.Sort,
        };
}
