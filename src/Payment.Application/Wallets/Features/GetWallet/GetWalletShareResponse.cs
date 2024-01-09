using System.Diagnostics.CodeAnalysis;

namespace Payment.Application.Wallets.Features.GetWallet;

[ExcludeFromCodeCoverage]
public sealed record GetWalletShareResponse
{
  public required string ShareId { get; init; }

  public required string WalletId { get; init; }

  public required string ContributorId { get; init; }

  public required string Currency { get; init; }

  public required decimal Amount { get; init; }

  public string? CreatedBy { get; init; }

  public DateTime? CreatedOn { get; init; }
}
