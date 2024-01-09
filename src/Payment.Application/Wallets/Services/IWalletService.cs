using OneOf;
using Optional;
using Payment.Application.Wallets.Features.ContributeWallet;
using Payment.Application.Wallets.Features.CreateWallet;
using Payment.Application.Wallets.Features.GetWallet;
using Payment.Common.Abstraction.Models;

namespace Payment.Application.Wallets.Services;

public interface IWalletService
{
  Task<OneOf<CreateWalletResponse, ValidationErrorResponse, ErrorResponse>> CreateWalletAsync(
    CreateWalletCommand command,
    CancellationToken cancellationToken = default);

  Task<OneOf<GetWalletResponse, ValidationErrorResponse, ErrorResponse>> GetWalletAsync(
    GetWalletQuery query,
    CancellationToken cancellationToken = default);

  Task<Option<ValidationErrorResponse, ErrorResponse>> ContributeWalletAsync(
    ContributeWalletCommand command,
    CancellationToken cancellationToken = default);
}
