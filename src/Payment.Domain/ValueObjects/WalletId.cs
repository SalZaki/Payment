using Ardalis.GuardClauses;

namespace Payment.Domain.ValueObjects;

public sealed record WalletId
{
  public required Guid Value { get; init; }

  public static WalletId Create(Guid value)
  {
    Guard.Against.Null(value, nameof(value), "Value can not be null.");

    return new WalletId {Value = value};
  }

  public override string ToString()
  {
    return Value.ToString();
  }

  public static implicit operator Guid(WalletId walletId) => walletId.Value;
}
