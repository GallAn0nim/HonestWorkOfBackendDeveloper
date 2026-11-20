namespace HonestWork.Api.Features.Auth;

public static class EditDistance
{
    public static int Between(string first, string second)
    {
        var previous = Enumerable.Range(0, second.Length + 1).ToArray();

        for (var row = 1; row <= first.Length; row++)
        {
            var current = new int[second.Length + 1];
            current[0] = row;

            for (var column = 1; column <= second.Length; column++)
            {
                var substitution = char.ToLowerInvariant(first[row - 1]) == char.ToLowerInvariant(second[column - 1]) ? 0 : 1;
                current[column] = Math.Min(Math.Min(previous[column] + 1, current[column - 1] + 1), previous[column - 1] + substitution);
            }

            previous = current;
        }

        return previous[second.Length];
    }
}
