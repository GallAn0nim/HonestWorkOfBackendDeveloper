namespace HonestWork.Api.Features.Sorting.Algorithms;

public static class BozoSort
{
    public static void Sort(SortRecorder items, Random random)
    {
        while (!items.IsSorted())
        {
            items.Swap(random.Next(items.Length), random.Next(items.Length));
        }
    }
}
