using System.Diagnostics.CodeAnalysis;

namespace Payment.Application.Users.Features.GetCommonFriends;

[ExcludeFromCodeCoverage]
public sealed record GetCommonFriendsResponse
{
  public required string UserId { get; init; }

  public required string FullName { get; init; }
}
