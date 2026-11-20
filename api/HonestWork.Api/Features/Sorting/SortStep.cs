namespace HonestWork.Api.Features.Sorting;

public record SortStep(string Type, int Index, int? OtherIndex = null, int? Value = null)
{
    public static SortStep Compare(int index, int otherIndex)
    {
        return new("compare", index, OtherIndex: otherIndex);
    }

    public static SortStep Write(int index, int value)
    {
        return new("write", index, Value: value);
    }

    public static SortStep Remove(int index)
    {
        return new("remove", index);
    }
}
