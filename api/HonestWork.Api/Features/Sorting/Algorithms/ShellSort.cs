namespace HonestWork.Api.Features.Sorting.Algorithms;

public static class ShellSort
{
    private static readonly int[] CiuraGaps = [701, 301, 132, 57, 23, 10, 4, 1];

    public static void Sort(SortRecorder items)
    {
        foreach (var gap in CiuraGaps.Where(gap => gap < items.Length))
        {
            for (var i = gap; i < items.Length; i++)
            {
                for (var j = i; j >= gap && items.Less(j, j - gap); j -= gap)
                {
                    items.Swap(j, j - gap);
                }
            }
        }
    }
}
