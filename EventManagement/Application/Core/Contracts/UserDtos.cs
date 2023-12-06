namespace EventManagement.Application.Core.Contracts;

public record RegisterUserRequest(string Name, string Email, string Password);

public record LoginUserRequest(string Email, string Password);

public record LoginUserResponse(string Token);