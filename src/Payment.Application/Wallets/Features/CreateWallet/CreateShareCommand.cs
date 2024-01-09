using System.Diagnostics.CodeAnalysis;
using Payment.Common.Abstraction.Commands;

namespace Payment.Application.Wallets.Features.CreateWallet;

[ExcludeFromCodeCoverage]
public sealed record CreateShareCommand : ICommand
{
  public required string ContributorId { get; init; }

  public required string Currency { get; init; }

  public required decimal Amount { get; init; }
}
