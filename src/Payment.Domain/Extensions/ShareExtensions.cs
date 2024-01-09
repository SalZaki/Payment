using Payment.Domain.Entities;
using Payment.Domain.ValueObjects;

namespace Payment.Domain.Extensions;

public static class ShareExtensions
{
  public static void Append(
    this IEnumerable<Share> shares,
    WalletId walletId,
    UserId contributorId,
    Money amount,
    string? modifiedBy = "System",
    DateTime? modifiedOn = null)
  {
    if (shares == null)
    {
      throw new ArgumentOutOfRangeException(nameof(shares), "One or more Share must be specified");
    }

    var share = shares.FirstOrDefault(x =>
      x.WalletId == walletId &&
      x.ContributorId == contributorId &&
      x.Amount.Currency.Code == amount.Currency.Code);

    if (share == null) return;

    share.AddAmount(amount);
    share.ModifiedBy = modifiedBy;
    share.ModifiedOn = modifiedOn ?? DateTime.UtcNow;
  }

  public static bool ContributorHasShares(
    this IEnumerable<Share> shares,
    WalletId walletId,
    UserId contributorId,
    Money amount)
  {
    if (shares == null)
    {
      throw new ArgumentOutOfRangeException(nameof(shares), "One or more Share must be specified");
    }

    // Checks if a contributor has shares in a given currency.
    return shares.Any(x =>
      x.WalletId == walletId &&
      x.ContributorId == contributorId &&
      x.Amount.Currency.Code == amount.Currency.Code);
  }
}
