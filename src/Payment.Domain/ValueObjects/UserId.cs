namespace Payment.Domain.ValueObjects;

public sealed record UserId
{
  public required Guid Value { get; init; }

  public static UserId Create(Guid value)
  {
//    Guard.Against.Null(value, nameof(value), "Value can not be null.");

    return new UserId {Value = value};
  }

  public override string ToString()
  {
    return Value.ToString();
  }

  public static implicit operator Guid(UserId userId) => userId.Value;
}
