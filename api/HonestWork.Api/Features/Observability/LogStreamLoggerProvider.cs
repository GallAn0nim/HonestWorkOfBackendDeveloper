using System.Diagnostics;

namespace HonestWork.Api.Features.Observability;

public sealed class LogStreamLoggerProvider(LogStream stream) : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName)
    {
        return new LogStreamLogger(stream, ModuleOf(categoryName));
    }

    public void Dispose()
    {
    }

    private static string ModuleOf(string category)
    {
        var parts = category.Split('.');
        var features = Array.IndexOf(parts, "Features");

        return features >= 0 && features + 1 < parts.Length ? parts[features + 1] : "API";
    }

    private static string LevelOf(LogLevel level)
    {
        return level switch
        {
            LogLevel.Warning => "WARN",
            LogLevel.Error or LogLevel.Critical => "ERROR",
            _ => "INFO",
        };
    }

    private sealed class LogStreamLogger(LogStream stream, string module) : ILogger
    {
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel != LogLevel.None;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            var message = formatter(state, null);
            var activity = Activity.Current;

            stream.Write(LevelOf(logLevel), module, exception is null ? message : $"{message} ({exception.GetType().Name})",
                activity?.TraceId.ToHexString(), activity?.SpanId.ToHexString());
        }
    }
}
