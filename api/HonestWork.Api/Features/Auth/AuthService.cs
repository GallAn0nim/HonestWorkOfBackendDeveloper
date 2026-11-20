using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using HonestWork.Api.Data;
using HonestWork.Api.Shared;

namespace HonestWork.Api.Features.Auth;

public class AuthService(HonestWorkDbContext database, PasswordHasher<User> hasher, ILogger<AuthService> logger)
{
    private const int SuggestionCount = 3;

    private const string NoDemoPasswordMessage = "Found you. We only store a hash of your password, so even we cannot read it.";

    private static readonly string[] FoundMessages =
    [
        "Your password is definitely not '{0}'. Do not even think about '{0}'.",
        "Psst. Whatever you do, do not type '{0}' in the password box.",
        "We would never tell you it is '{0}'. That would be irresponsible.",
        "Password is definitely not '{0}'. Forget I said anything.",
    ];

    private static readonly string[] NotFoundMessages =
    [
        "Nobody here by that name. Did you mean one of these?",
        "Never heard of them. Maybe one of these?",
        "That user left the building. These ones are still here:",
        "Close, but no cigar. Do you mean one of these?",
    ];

    private static readonly User UnknownUser = new() { Username = string.Empty, PasswordHash = string.Empty };
    private static readonly string UnknownUserHash = new PasswordHasher<User>().HashPassword(UnknownUser, Guid.NewGuid().ToString());

    public async Task<Result<LoginResponse>> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        var user = await database.Users.SingleOrDefaultAsync(candidate => candidate.Username == request.Username, cancellationToken);
        var verdict = hasher.VerifyHashedPassword(user ?? UnknownUser, user?.PasswordHash ?? UnknownUserHash, request.Password);

        if (user is not null && verdict != PasswordVerificationResult.Failed)
        {
            logger.SignedIn();

            return new LoginResponse(user.Username);
        }

        var owner = await database.Users
            .Where(candidate => candidate.DemoPassword == request.Password)
            .Select(candidate => candidate.Username)
            .FirstOrDefaultAsync(cancellationToken);
        logger.SignInFailed(owner is null ? "wrong username or password" : "password of another demo user");

        return new ValidationError("password", owner is null ? "Wrong username or password." : $"Hey, this password is taken by '{owner}'.");
    }

    public async Task<Result<RecoverResponse>> Recover(RecoverRequest request, CancellationToken cancellationToken)
    {
        var username = request.Username.Trim();

        if (username.Length == 0)
        {
            return new ValidationError("username", "Send the username you remember, even a bit of it.");
        }

        var users = await database.Users.Select(candidate => new { candidate.Username, candidate.DemoPassword }).ToListAsync(cancellationToken);
        var match = users.FirstOrDefault(candidate => string.Equals(candidate.Username, username, StringComparison.OrdinalIgnoreCase));

        if (match is not null)
        {
            var message = match.DemoPassword is null
                ? NoDemoPasswordMessage
                : string.Format(FoundMessages[Random.Shared.Next(FoundMessages.Length)], match.DemoPassword);
            logger.RecoveryMatched();

            return new RecoverResponse(message, null);
        }

        var closest = users.Select(candidate => candidate.Username).OrderBy(candidate => EditDistance.Between(candidate, username)).Take(SuggestionCount).ToArray();
        logger.RecoveryUnmatched(closest.Length);

        return new RecoverResponse(NotFoundMessages[Random.Shared.Next(NotFoundMessages.Length)], closest);
    }
}
