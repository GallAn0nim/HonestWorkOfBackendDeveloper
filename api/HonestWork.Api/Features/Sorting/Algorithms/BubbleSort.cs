namespace HonestWork.Api.Features.Sorting.Algorithms;

public static class BubbleSort
{
    public static void Sort(SortRecorder items)
    {
        for (var end = items.Length - 1; end > 0; end--)
        {
            var swapped = false;

            for (var i = 0; i < end; i++)
            {
                if (items.Compare(i, i + 1) > 0)
                {
                    items.Swap(i, i + 1);
                    swapped = true;
                }
            }

            if (!swapped)
            {
                return;
            }
        }
    }
}
