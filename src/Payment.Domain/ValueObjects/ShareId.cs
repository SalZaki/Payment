using Payment.Domain.Exceptions;

namespace Payment.Domain.ValueObjects;

public sealed record ShareId
{
  public required Guid Value { get; init; }

  public static ShareId Create(Guid value)
  {
    if (value == Guid.Empty || value == default)
    {
      throw new InvalidShareIdException(value);
    }

    return new ShareId {Value = value};
  }

  public override string ToString()
  {
    return Value.ToString();
  }

  public static implicit operator Guid(ShareId shareId) => shareId.Value;
}
