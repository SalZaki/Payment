using System.Collections.Immutable;
using Ardalis.GuardClauses;
using Payment.Common.Abstraction.Domain;
using Payment.Domain.Extensions;
using Payment.Domain.Policies;
using Payment.Domain.ValueObjects;

namespace Payment.Domain.Entities;

public sealed record Wallet : Aggregate<WalletId>, IComparable<Wallet>, IComparable
{
  private readonly List<Share> _share = new();

  public required UserId UserId { get; init; }

  public required Money Amount { get; init; }

  public ImmutableHashSet<Share> Shares => _share.ToImmutableHashSet();

  public IDictionary<string, decimal> TotalSharesAmount => Shares
    .Select(x => x.Amount)
    .SumCurrencies();

  public static readonly Wallet NotFound = Create(WalletId.Create(Guid.Empty), UserId.Create(Guid.Empty));

  private Wallet(WalletId walletId) : base(walletId)
  {
  }

  public static Wallet Create(
    WalletId walletId,
    UserId userId,
    Money? amount = null,
    string? createdBy = "System",
    DateTime? createdOn = default)
  {
    Guard.Against.Null(walletId, nameof(walletId), "WalletId can not be null.");
    Guard.Against.Null(userId, nameof(userId), "UserId can not be null.");

    var wallet = new Wallet(walletId)
    {
      UserId = userId,
      Amount = amount ?? Money.Empty(),
      CreatedBy = createdBy,
      CreatedOn = createdOn ?? DateTime.UtcNow
    };

    return wallet;
  }

  public void Contribute(
    Money amount,
    UserId contributorId,
    string? createdOrModifiedBy = "System",
    DateTime? createdOrModifiedOn = default)
  {
    CheckPolicy(new UserCannotContributeToOwnWallet(UserId, contributorId));

    if (Shares.ContributorHasShares(Id, contributorId, amount))
    {
      AppendShare(amount, contributorId, createdOrModifiedBy!, createdOrModifiedOn ?? DateTime.UtcNow);
    }
    else
    {
      AddShare(amount, contributorId, createdOrModifiedBy!, createdOrModifiedOn ?? DateTime.UtcNow);
    }
  }

  public int CompareTo(Wallet? other)
  {
    if (other is null) return 1;

    return Id == other.Id ? 0 : 1;
  }

  public int CompareTo(object? obj)
  {
    if (obj is null) return 1;

    return obj is not Wallet wallet ? 1 : CompareTo(wallet);
  }

  private void AppendShare(Money amount, UserId contributorId, string modifiedBy, DateTime modifiedOn)
  {
    _share.Append(Id, contributorId, amount, modifiedBy, modifiedOn);
  }

  private void AddShare(Money amount, UserId contributorId, string createdBy, DateTime createdOn)
  {
    var shareId = ShareId.Create(Guid.NewGuid());

    var share = Share.Create(shareId, Id, contributorId, amount, createdBy, createdOn);

    _share.Add(share);
  }
}
