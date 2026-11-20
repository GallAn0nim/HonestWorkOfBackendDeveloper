namespace HonestWork.Api.Features.Sorting.Algorithms;

public static class PurgeSort
{
    public static void Sort(SortRecorder items)
    {
        var lastKept = -1;

        for (var i = 0; i < items.Length; i++)
        {
            if (lastKept < 0 || items.Compare(i, lastKept) >= 0)
            {
                lastKept = i;
                items.Write(i, items.ValueAt(i));
            }
            else
            {
                items.Remove(i);
            }
        }
    }
}
