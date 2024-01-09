using System.Diagnostics.CodeAnalysis;

namespace Payment.Application.Users.Features.AddFriendship;

[ExcludeFromCodeCoverage]
public sealed record AddFriendshipResponse
{
  public required string FriendId { get; init; }
}
