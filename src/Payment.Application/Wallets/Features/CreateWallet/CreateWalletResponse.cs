namespace Payment.Application.Wallets.Features.CreateWallet;

public sealed record CreateWalletResponse
{
  public required string WalletId { get; init; }
}
