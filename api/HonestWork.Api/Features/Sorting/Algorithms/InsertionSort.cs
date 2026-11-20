namespace HonestWork.Api.Features.Sorting.Algorithms;

public static class InsertionSort
{
    public static void Sort(SortRecorder items)
    {
        for (var i = 1; i < items.Length; i++)
        {
            for (var j = i; j > 0 && items.Less(j, j - 1); j--)
            {
                items.Swap(j, j - 1);
            }
        }
    }
}
