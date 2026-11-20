namespace HonestWork.Api.Features.Auth;

public class User
{
    public Guid Id { get; init; }
    public required string Username { get; init; }
    public required string PasswordHash { get; set; }
    public string? DemoPassword { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
}
