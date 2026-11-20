namespace HonestWork.Api.Features.Sorting.Algorithms;

public static class SelectionSort
{
    public static void Sort(SortRecorder items)
    {
        for (var i = 0; i < items.Length - 1; i++)
        {
            var smallest = i;

            for (var j = i + 1; j < items.Length; j++)
            {
                if (items.Less(j, smallest))
                {
                    smallest = j;
                }
            }

            if (smallest != i)
            {
                items.Swap(i, smallest);
            }
        }
    }
}
