namespace HonestWork.Api.Features.Sorting.Algorithms;

public static class CycleSort
{
    public static void Sort(SortRecorder items)
    {
        for (var cycleStart = 0; cycleStart < items.Length - 1; cycleStart++)
        {
            var item = items.ValueAt(cycleStart);
            var position = FindPosition(items, item, cycleStart);

            if (position == cycleStart)
            {
                continue;
            }

            while (position != cycleStart)
            {
                position = SkipEqual(items, item, cycleStart, FindPosition(items, item, cycleStart));

                if (items.CompareValues(item, items.ValueAt(position), cycleStart, position) != 0)
                {
                    var displaced = items.ValueAt(position);
                    items.Write(position, item);
                    item = displaced;
                }
            }
        }
    }

    private static int FindPosition(SortRecorder items, int item, int cycleStart)
    {
        var position = cycleStart;

        for (var i = cycleStart + 1; i < items.Length; i++)
        {
            if (items.CompareValues(items.ValueAt(i), item, i, cycleStart) < 0)
            {
                position++;
            }
        }

        return position;
    }

    private static int SkipEqual(SortRecorder items, int item, int cycleStart, int position)
    {
        while (position != cycleStart && items.CompareValues(item, items.ValueAt(position), cycleStart, position) == 0)
        {
            position++;
        }

        return position;
    }
}
