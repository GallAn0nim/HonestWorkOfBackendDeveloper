namespace HonestWork.Api.Features.Cv;

public class CvOptions
{
    public const string Section = "Cv";

    public string DefaultLanguage { get; init; } = string.Empty;
    public CvPageOptions Page { get; init; } = new();
    public Dictionary<string, CvLabels> Labels { get; init; } = [];
}

public class CvPageOptions
{
    public float WidthMm { get; init; }
    public float HeightMm { get; init; }
    public float Margin { get; init; }
    public float FontSize { get; init; }
}

public class CvLabels
{
    public string Skills { get; init; } = string.Empty;
    public string Experience { get; init; } = string.Empty;
    public string Education { get; init; } = string.Empty;
    public string Languages { get; init; } = string.Empty;
    public string Thesis { get; init; } = string.Empty;
    public string Technologies { get; init; } = string.Empty;
    public string Present { get; init; } = string.Empty;
}
