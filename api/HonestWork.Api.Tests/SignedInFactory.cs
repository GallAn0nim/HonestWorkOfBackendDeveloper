using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HonestWork.Api.Tests;

public class SignedInFactory : WebApplicationFactory<Program>
{
    public const string Scheme = "Test";
    public const string Username = "tester";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services => services
            .AddAuthentication(Scheme)
            .AddScheme<AuthenticationSchemeOptions, SignedInHandler>(Scheme, null));
    }
}

public class SignedInHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        Claim[] claims = [new(ClaimTypes.Name, SignedInFactory.Username)];
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, SignedInFactory.Scheme));

        return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(principal, SignedInFactory.Scheme)));
    }
}
