using HonestWork.Api.Features.Auth;

namespace HonestWork.Api.Tests;

public class EditDistanceTests
{
    [Theory]
    [InlineData("oskar", "oskar", 0)]
    [InlineData("oskar", "OSKAR", 0)]
    [InlineData("oskar", "oskr", 1)]
    [InlineData("admin", "administrator", 8)]
    [InlineData("", "test", 4)]
    public void Between_CountsSingleCharacterEdits(string first, string second, int expected)
    {
        Assert.Equal(expected, EditDistance.Between(first, second));
    }
}
