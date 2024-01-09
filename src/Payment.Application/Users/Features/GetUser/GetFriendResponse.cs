using System.Diagnostics.CodeAnalysis;

namespace Payment.Application.Users.Features.GetUser;

[ExcludeFromCodeCoverage]
public sealed record GetFriendshipResponse
{
  public required string FriendId { get; init; }

  public required string UserId { get; init; }

  public required GetUserResponse Friend { get; init; }
}
