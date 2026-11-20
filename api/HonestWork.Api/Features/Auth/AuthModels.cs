namespace HonestWork.Api.Features.Auth;

public record LoginRequest(string Username, string Password);
public record LoginResponse(string Username);
public record RecoverRequest(string Username);
public record RecoverResponse(string Message, string[]? Suggestions);
