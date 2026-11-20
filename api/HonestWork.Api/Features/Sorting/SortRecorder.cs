namespace HonestWork.Api.Features.Sorting;

public class SortRecorder
{
    private readonly int?[] items;
    private readonly int maxSteps;
    private readonly List<SortStep> steps = [];

    private SortRecorder(int?[] items, int maxSteps)
    {
        this.items = items;
        this.maxSteps = maxSteps;
    }

    public int Length => items.Length;

    public static List<SortStep> Record(int?[] items, Action<SortRecorder> algorithm, int maxSteps)
    {
        var recorder = new SortRecorder(items, maxSteps);
        try
        {
            algorithm(recorder);
        }
        catch (StepLimitReachedException)
        {
        }

        return recorder.steps;
    }

    public int ValueAt(int index)
    {
        return items[index]!.Value;
    }

    public int Compare(int index, int otherIndex)
    {
        return CompareValues(ValueAt(index), ValueAt(otherIndex), index, otherIndex);
    }

    public bool Less(int index, int otherIndex)
    {
        return Compare(index, otherIndex) < 0;
    }

    public int CompareValues(int value, int otherValue, int index, int otherIndex)
    {
        Add(SortStep.Compare(index, otherIndex));
        return value.CompareTo(otherValue);
    }

    public void Write(int index, int value)
    {
        Add(SortStep.Write(index, value));
        items[index] = value;
    }

    public void Swap(int index, int otherIndex)
    {
        var value = ValueAt(index);
        Write(index, ValueAt(otherIndex));
        Write(otherIndex, value);
    }

    public void Remove(int index)
    {
        Add(SortStep.Remove(index));
        items[index] = null;
    }

    public bool IsSorted()
    {
        for (var i = 1; i < Length; i++)
        {
            if (Less(i, i - 1))
            {
                return false;
            }
        }

        return true;
    }

    private void Add(SortStep step)
    {
        if (steps.Count == maxSteps)
        {
            throw new StepLimitReachedException();
        }

        steps.Add(step);
    }

    private class StepLimitReachedException : Exception;
}
