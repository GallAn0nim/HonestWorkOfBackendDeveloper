namespace HonestWork.Api.Features.Cv;

internal static partial class CvLogs
{
    [LoggerMessage(EventId = 7001, Level = LogLevel.Information, Message = "CV rendered in {Language} ({SizeBytes} bytes)")]
    public static partial void Rendered(this ILogger logger, string language, int sizeBytes);

    [LoggerMessage(EventId = 7002, Level = LogLevel.Warning, Message = "No CV profile for language {Language}")]
    public static partial void ProfileMissing(this ILogger logger, string language);
}
