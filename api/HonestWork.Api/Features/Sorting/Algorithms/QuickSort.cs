namespace HonestWork.Api.Features.Sorting.Algorithms;

public static class QuickSort
{
    public static void Sort(SortRecorder items)
    {
        Sort(items, 0, items.Length - 1);
    }

    private static void Sort(SortRecorder items, int low, int high)
    {
        if (low >= high)
        {
            return;
        }

        var pivot = Partition(items, low, high);
        Sort(items, low, pivot - 1);
        Sort(items, pivot + 1, high);
    }

    private static int Partition(SortRecorder items, int low, int high)
    {
        var boundary = low;

        for (var i = low; i < high; i++)
        {
            if (items.Less(i, high))
            {
                if (i != boundary)
                {
                    items.Swap(i, boundary);
                }

                boundary++;
            }
        }

        if (boundary != high)
        {
            items.Swap(boundary, high);
        }

        return boundary;
    }
}
