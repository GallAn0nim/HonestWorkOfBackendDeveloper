using System.Text.Json.Serialization;
using HonestWork.Api.Data;
using HonestWork.Api.Features.Auth;
using HonestWork.Api.Features.Cache;
using HonestWork.Api.Features.Cv;
using HonestWork.Api.Features.Mazes;
using HonestWork.Api.Features.Observability;
using HonestWork.Api.Features.Quotes;
using HonestWork.Api.Features.Race;
using HonestWork.Api.Features.Sorting;
using HonestWork.Api.Shared;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<KestrelServerOptions>(builder.Configuration.GetSection("Kestrel"));
builder.Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull);
builder.Services.AddOpenApi();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.Cookie.SecurePolicy = builder.Environment.IsDevelopment() ? CookieSecurePolicy.SameAsRequest : CookieSecurePolicy.Always;
        options.Events.OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        };
    });
builder.Services.AddAuthorization();
builder.Services.AddApiRateLimiting(builder.Configuration);
builder.Services.Configure<ForwardedHeadersOptions>(options => options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto);
builder.Services.Configure<SortingOptions>(builder.Configuration.GetSection(SortingOptions.Section));
builder.Services.Configure<MazeOptions>(builder.Configuration.GetSection(MazeOptions.Section));
builder.Services.Configure<CvOptions>(builder.Configuration.GetSection(CvOptions.Section));
builder.Services.Configure<CacheOptions>(builder.Configuration.GetSection(CacheOptions.Section));
builder.Services.Configure<RaceOptions>(builder.Configuration.GetSection(RaceOptions.Section));
builder.Services.Configure<ObservabilityOptions>(builder.Configuration.GetSection(ObservabilityOptions.Section));
builder.Services.AddHybridCache();
builder.Services.AddSingleton<SortingService>();
builder.Services.AddSingleton<MazeService>();
builder.Services.AddSingleton<CacheService>();
builder.Services.AddSingleton<RaceService>();
var connectionString = builder.Configuration.GetConnectionString("HonestWork")
    ?? throw new InvalidOperationException("Set ConnectionStrings:HonestWork in user secrets or the ConnectionStrings__HonestWork environment variable.");
builder.Services.AddDbContext<HonestWorkDbContext>(options => options.UseNpgsql(connectionString));
builder.Services.AddSingleton<PasswordHasher<User>>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<CvService>();
builder.Services.AddScoped<QuoteService>();
builder.Services.AddSingleton<LogStream>();
builder.Services.AddSingleton<ILoggerProvider, LogStreamLoggerProvider>();
builder.Services.AddSingleton<RequestMetrics>();
builder.Services.AddSingleton<RaceTraces>();
builder.Logging.AddFilter<LogStreamLoggerProvider>(null, LogLevel.None);
builder.Logging.AddFilter<LogStreamLoggerProvider>("HonestWork", LogLevel.Information);
builder.Logging.AddFilter<LogStreamLoggerProvider>("Microsoft.AspNetCore.Diagnostics", LogLevel.Error);
builder.Logging.AddFilter<LogStreamLoggerProvider>("Microsoft.AspNetCore.Server.Kestrel", LogLevel.Error);

if (!builder.Environment.IsDevelopment())
{
    builder.Logging.AddJsonConsole(options =>
    {
        options.IncludeScopes = true;
        options.UseUtcTimestamp = true;
    });
}

var app = builder.Build();
app.Services.GetRequiredService<RequestMetrics>();
app.Services.GetRequiredService<RaceTraces>();
app.UseForwardedHeaders();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    scope.ServiceProvider.GetRequiredService<HonestWorkDbContext>().Database.Migrate();

    app.MapOpenApi();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "HonestWork API"));
}
else
{
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

public partial class Program;
