using Payment.Common.Abstraction.Domain;
using Payment.Domain.ValueObjects;

namespace Payment.Domain.Entities;

public sealed record Friendship : Entity<FriendId>, IComparable<Friendship>, IComparable
{
  public required UserId UserId { get; init; }

  public required User Friend { get; init; }

  private Friendship(FriendId Id) : base(Id)
  {
  }

  public static Friendship Create(FriendId friendId, UserId userId, User friend)
  {
    return new Friendship(friendId)
    {
      UserId = userId,
      Friend = friend
    };
  }

  public int CompareTo(Friendship? other)
  {
    if (other is null) return 1;

    return Id == other.Id && UserId == other.UserId ? 0 : 1;
  }

  public int CompareTo(object? obj)
  {
    if (obj is null) return 1;

    return obj is not Friendship friendship ? 1 : CompareTo(friendship);
  }
}
