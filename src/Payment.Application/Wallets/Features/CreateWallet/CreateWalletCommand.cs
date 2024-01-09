using System.Diagnostics.CodeAnalysis;
using Payment.Common.Abstraction.Commands;

namespace Payment.Application.Wallets.Features.CreateWallet;

[ExcludeFromCodeCoverage]
public sealed record CreateWalletCommand : ICommand
{
  public required string UserId { get; init; }

  public string? Currency { get; init; }

  public decimal? Amount { get; init; }
}
