namespace HonestWork.Api.Features.Mazes;

public record GenerationStep(string Type, int Cell, Walls Side)
{
    public static GenerationStep Carve(int cell, Walls side)
    {
        return new("carve", cell, side);
    }

    public static GenerationStep Open(int cell, Walls side)
    {
        return new("open", cell, side);
    }

    public static GenerationStep Door(int cell, Walls side)
    {
        return new("door", cell, side);
    }
}
