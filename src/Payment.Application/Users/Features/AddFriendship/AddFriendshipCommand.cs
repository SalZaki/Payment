using System.Diagnostics.CodeAnalysis;
using Payment.Common.Abstraction.Commands;

namespace Payment.Application.Users.Features.AddFriendship;

[ExcludeFromCodeCoverage]
public sealed record AddFriendshipCommand : ICommand
{
  public required string UserId { get; init; }

  public required string FriendId { get; init; }
}
