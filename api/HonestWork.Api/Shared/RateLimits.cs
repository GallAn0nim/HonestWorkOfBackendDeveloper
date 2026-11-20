using System.Threading.RateLimiting;
using Microsoft.Extensions.Options;

namespace HonestWork.Api.Shared;

public class RateLimitOptions
{
    public const string Section = "RateLimiting";

    public FixedWindowLimit General { get; init; } = new() { PermitLimit = 500, WindowSeconds = 60 };
    public FixedWindowLimit Auth { get; init; } = new() { PermitLimit = 75, WindowSeconds = 60 };
    public FixedWindowLimit Heavy { get; init; } = new() { PermitLimit = 150, WindowSeconds = 60 };
    public int LogStreamConnections { get; init; } = 4;
}

public class FixedWindowLimit
{
    public int PermitLimit { get; init; }
    public int WindowSeconds { get; init; }
}

public static class RateLimits
{
    public const string Auth = "auth";
    public const string Heavy = "heavy";
    public const string LogStream = "log-stream";

    private const string UnknownClient = "unknown";

    public static IServiceCollection AddApiRateLimiting(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RateLimitOptions>(configuration.GetSection(RateLimitOptions.Section));

        return services.AddRateLimiter(limiter =>
        {
            limiter.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            limiter.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context => FixedWindow(context, Settings(context).General));
            limiter.AddPolicy(Auth, context => FixedWindow(context, Settings(context).Auth));
            limiter.AddPolicy(Heavy, context => FixedWindow(context, Settings(context).Heavy));
            limiter.AddPolicy(LogStream, context => RateLimitPartition.GetConcurrencyLimiter(ClientOf(context), _ => new ConcurrencyLimiterOptions
            {
                PermitLimit = Settings(context).LogStreamConnections,
                QueueLimit = 0,
            }));
        });
    }

    private static RateLimitPartition<string> FixedWindow(HttpContext context, FixedWindowLimit limit)
    {
        return RateLimitPartition.GetFixedWindowLimiter(ClientOf(context), _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = limit.PermitLimit,
            Window = TimeSpan.FromSeconds(limit.WindowSeconds),
            QueueLimit = 0,
        });
    }

    private static RateLimitOptions Settings(HttpContext context)
    {
        return context.RequestServices.GetRequiredService<IOptions<RateLimitOptions>>().Value;
    }

    private static string ClientOf(HttpContext context)
    {
        return context.Connection.RemoteIpAddress?.ToString() ?? UnknownClient;
    }
}
