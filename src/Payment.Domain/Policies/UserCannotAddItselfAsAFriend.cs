using Payment.Common.Abstraction.Domain;
using Payment.Domain.ValueObjects;

namespace Payment.Domain.Policies;

public sealed record UserCannotAddItselfAsAFriend(UserId UserId, UserId FriendId) : IBusinessPolicy
{
  public string Message => $"User with id {UserId} cannot add itself as a friend.";

  public bool IsInvalid()
  {
    return UserId == FriendId;
  }
}
