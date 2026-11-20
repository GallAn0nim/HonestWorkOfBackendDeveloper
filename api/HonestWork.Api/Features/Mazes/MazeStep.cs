namespace HonestWork.Api.Features.Mazes;

public record MazeStep(string Type, int Index)
{
    public const string VisitType = "visit";
    public const string PathType = "path";

    public static MazeStep Visit(int index)
    {
        return new(VisitType, index);
    }

    public static MazeStep Path(int index)
    {
        return new(PathType, index);
    }
}
