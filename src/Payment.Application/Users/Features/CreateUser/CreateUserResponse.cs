namespace Payment.Application.Users.Features.CreateUser;

public sealed record CreateUserResponse
{
  public required string UserId { get; init; }
}
