using System.Diagnostics.CodeAnalysis;
using Payment.Common.Abstraction.Commands;

namespace Payment.Application.Wallets.Features.ContributeWallet;

[ExcludeFromCodeCoverage]
public sealed record ContributeWalletCommand : ICommand
{
  public required string WalletId { get; init; }

  public required IEnumerable<ContributeShareCommand> Shares { get; init; }

  public string? CreatedOrModifiedBy { get; init; }

  public DateTime? CreatedOrModifiedOn { get; init; }
}
