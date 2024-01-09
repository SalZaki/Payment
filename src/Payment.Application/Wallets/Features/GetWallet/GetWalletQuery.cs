using OneOf;
using System.Diagnostics.CodeAnalysis;
using Payment.Common.Abstraction.Models;
using Payment.Common.Abstraction.Queries;

namespace Payment.Application.Wallets.Features.GetWallet;

[ExcludeFromCodeCoverage]
public sealed record GetWalletQuery : IQuery<OneOf<GetWalletResponse, ValidationErrorResponse, ErrorResponse>>
{
  public required string WalletId { get; init; }
}
