namespace TaskManager.Application.Users;

public sealed record RegisterUserRequest(string Email, string Password);
public sealed record LoginUserRequest(string Email, string Password);
public sealed record AuthResponse(Guid UserId, string Email, string AccessToken);
public sealed record UserResponse(Guid Id, string Email, DateTime CreatedAtUtc);
