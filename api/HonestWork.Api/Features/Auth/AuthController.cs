using System.Security.Claims;
using HonestWork.Api.Shared;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace HonestWork.Api.Features.Auth;

[ApiController]
[Route("api/auth")]
public class AuthController(AuthService service, ILogger<AuthController> logger) : ControllerBase
{
    [HttpPost("login")]
    [EnableRateLimiting(RateLimits.Auth)]
    public async Task<ActionResult<LoginResponse>> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await service.Login(request, cancellationToken);

        if (result.IsFailure)
        {
            return this.Invalid(result.Error);
        }

        Claim[] claims = [new(ClaimTypes.Name, result.Value.Username)];
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(new ClaimsPrincipal(identity));

        return Ok(result.Value);
    }

    [Authorize]
    [HttpGet("me")]
    public ActionResult<LoginResponse> Me()
    {
        return new LoginResponse(User.FindFirstValue(ClaimTypes.Name)!);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        logger.SignedOut();

        return NoContent();
    }

    [HttpPost("recover")]
    [EnableRateLimiting(RateLimits.Auth)]
    public async Task<ActionResult<RecoverResponse>> Recover(RecoverRequest request, CancellationToken cancellationToken)
    {
        var result = await service.Recover(request, cancellationToken);

        if (result.IsFailure)
        {
            return this.Invalid(result.Error);
        }

        return Ok(result.Value);
    }
}
