using System.Diagnostics.CodeAnalysis;

namespace Payment.Application.Wallets.Features.GetWallet;

[ExcludeFromCodeCoverage]
public sealed record GetWalletResponse
{
  public required string WalletId { get; init; }

  public required string UserId { get; init; }

  public required decimal Amount { get; init; }

  public required string Currency { get; init; }

  public IReadOnlyCollection<GetWalletShareResponse>? Shares { get; init; }

  public IReadOnlyDictionary<string, decimal>? TotalSharesAmount { get; init; }

  public string? CreatedBy { get; init; }

  public DateTime? CreatedOn { get; init; }
}
