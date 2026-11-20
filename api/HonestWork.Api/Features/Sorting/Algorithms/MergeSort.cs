namespace HonestWork.Api.Features.Sorting.Algorithms;

public static class MergeSort
{
    public static void Sort(SortRecorder items)
    {
        Sort(items, 0, items.Length);
    }

    private static void Sort(SortRecorder items, int start, int end)
    {
        if (end - start < 2)
        {
            return;
        }

        var middle = (start + end) / 2;
        Sort(items, start, middle);
        Sort(items, middle, end);
        Merge(items, start, middle, end);
    }

    private static void Merge(SortRecorder items, int start, int middle, int end)
    {
        var left = Enumerable.Range(start, middle - start).Select(items.ValueAt).ToArray();
        int leftIndex = 0, rightIndex = middle, target = start;

        while (leftIndex < left.Length && rightIndex < end)
        {
            if (items.CompareValues(left[leftIndex], items.ValueAt(rightIndex), target, rightIndex) <= 0)
            {
                items.Write(target++, left[leftIndex++]);
            }
            else
            {
                items.Write(target++, items.ValueAt(rightIndex++));
            }
        }

        while (leftIndex < left.Length)
        {
            items.Write(target++, left[leftIndex++]);
        }
    }
}
