using Ardalis.GuardClauses;
using Payment.Common.Abstraction.Domain;
using Payment.Domain.ValueObjects;

namespace Payment.Domain.Entities;

public sealed record Share : Entity<ShareId>, IComparable<Share>, IComparable
{
  public required UserId ContributorId { get; init; }

  public required WalletId WalletId { get; init; }

  public Money Amount { get; private set; }

  private Share(ShareId shareId, Money amount) : base(shareId)
  {
    Amount = amount;
  }

  public static Share Create(
    ShareId shareId,
    WalletId walletId,
    UserId contributorId,
    Money amount,
    string? createdBy = "System",
    DateTime? createdOn = default)
  {
    Guard.Against.Null(shareId, nameof(shareId), "ShareId can not be null.");
    Guard.Against.Null(amount, nameof(amount), "Amount can not be null.");

    var share = new Share(shareId, amount)
    {
      ContributorId = Guard.Against.Null(contributorId, nameof(contributorId), "ContributorId can not be null."),
      WalletId = Guard.Against.Null(walletId, nameof(walletId), "WalletId can not be null."),
      CreatedBy = createdBy,
      CreatedOn = createdOn ?? DateTime.UtcNow
    };

    return share;
  }

  public void AddAmount(Money amount)
  {
    Guard.Against.Default(amount, nameof(amount), "Amount can not be null.");

    Amount += amount;

    // Raise Domain event called AmountIsAdded
  }

  public int CompareTo(Share? other)
  {
    if (other is null) return 1;

    return Id == other.Id ? 0 : 1;
  }

  public int CompareTo(object? obj)
  {
    if (obj is null) return 1;

    return obj is not Share share ? 1 : CompareTo(share);
  }
}
