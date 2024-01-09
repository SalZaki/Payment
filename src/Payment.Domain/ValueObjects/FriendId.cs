using Ardalis.GuardClauses;

namespace Payment.Domain.ValueObjects;

public sealed record FriendId
{
  public required Guid Value { get; init; }

  public static FriendId Create(Guid value)
  {
    Guard.Against.Null(value, nameof(value), "Value can not be null.");

    return new FriendId {Value = value};
  }

  public override string ToString()
  {
    return Value.ToString();
  }

  public static implicit operator Guid(FriendId friendId) => friendId.Value;
}
