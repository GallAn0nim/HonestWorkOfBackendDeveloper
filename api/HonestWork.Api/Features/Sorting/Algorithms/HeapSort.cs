namespace HonestWork.Api.Features.Sorting.Algorithms;

public static class HeapSort
{
    public static void Sort(SortRecorder items)
    {
        for (var root = items.Length / 2 - 1; root >= 0; root--)
        {
            SiftDown(items, root, items.Length);
        }

        for (var last = items.Length - 1; last > 0; last--)
        {
            items.Swap(0, last);
            SiftDown(items, 0, last);
        }
    }

    private static void SiftDown(SortRecorder items, int root, int count)
    {
        while (2 * root + 1 < count)
        {
            var child = 2 * root + 1;

            if (child + 1 < count && items.Less(child, child + 1))
            {
                child++;
            }

            if (!items.Less(root, child))
            {
                return;
            }

            items.Swap(root, child);
            root = child;
        }
    }
}
