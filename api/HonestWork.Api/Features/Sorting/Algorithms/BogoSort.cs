namespace HonestWork.Api.Features.Sorting.Algorithms;

public static class BogoSort
{
    public static void Sort(SortRecorder items, Random random)
    {
        while (!items.IsSorted())
        {
            for (var i = items.Length - 1; i > 0; i--)
            {
                items.Swap(i, random.Next(i + 1));
            }
        }
    }
}
