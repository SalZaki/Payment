using System.Diagnostics.CodeAnalysis;

namespace Payment.Application.Users.Features.GetUser;

[ExcludeFromCodeCoverage]
public sealed record GetUserResponse
{
  public required string UserId { get; init; }

  public required string FullName { get; init; }

  public IReadOnlyCollection<GetFriendshipResponse>? Friendships { get; init; }

  public string? CreatedBy { get; init; }

  public DateTime? CreatedOn { get; init; }
}
