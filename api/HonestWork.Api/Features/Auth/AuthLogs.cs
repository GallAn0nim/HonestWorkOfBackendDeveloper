namespace HonestWork.Api.Features.Auth;

internal static partial class AuthLogs
{
    [LoggerMessage(EventId = 5001, Level = LogLevel.Information, Message = "Sign-in succeeded")]
    public static partial void SignedIn(this ILogger logger);

    [LoggerMessage(EventId = 5002, Level = LogLevel.Warning, Message = "Sign-in failed: {Reason}")]
    public static partial void SignInFailed(this ILogger logger, string reason);

    [LoggerMessage(EventId = 5003, Level = LogLevel.Information, Message = "Signed out")]
    public static partial void SignedOut(this ILogger logger);

    [LoggerMessage(EventId = 5004, Level = LogLevel.Information, Message = "Password recovery matched a demo user")]
    public static partial void RecoveryMatched(this ILogger logger);

    [LoggerMessage(EventId = 5005, Level = LogLevel.Information, Message = "Password recovery matched no user, suggested {SuggestionCount} usernames")]
    public static partial void RecoveryUnmatched(this ILogger logger, int suggestionCount);
}
