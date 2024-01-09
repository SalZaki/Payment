using Payment.Common.Abstraction.Commands;

namespace Payment.Application.Wallets.Features.ContributeWallet;

public sealed record ContributeShareCommand : ICommand
{
  public required string ContributorId { get; init; }

  public required string Currency { get; init; }

  public required decimal Amount { get; init; }
}
