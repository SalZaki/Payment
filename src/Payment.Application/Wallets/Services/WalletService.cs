using Ardalis.GuardClauses;
using OneOf;
using Optional;
using Payment.Application.Wallets.Features.ContributeWallet;
using Payment.Application.Wallets.Features.CreateWallet;
using Payment.Application.Wallets.Features.GetWallet;
using Payment.Common.Abstraction.Commands;
using Payment.Common.Abstraction.Models;
using Payment.Common.Abstraction.Queries;

namespace Payment.Application.Wallets.Services;

public sealed class WalletService(
  ICommandHandler<CreateWalletCommand, OneOf<CreateWalletResponse, ValidationErrorResponse, ErrorResponse>> createWalletCommandHandler,
  IQueryHandler<GetWalletQuery, OneOf<GetWalletResponse, ValidationErrorResponse, ErrorResponse>> getWalletQueryHandler,
  ICommandHandler<ContributeWalletCommand, Option<ValidationErrorResponse, ErrorResponse>> contributeWalletCommandHandler) : IWalletService
{
  private readonly ICommandHandler<CreateWalletCommand, OneOf<CreateWalletResponse, ValidationErrorResponse, ErrorResponse>> _createWalletCommandHandler =
    Guard.Against.Null(createWalletCommandHandler, nameof(createWalletCommandHandler));

  private readonly IQueryHandler<GetWalletQuery, OneOf<GetWalletResponse, ValidationErrorResponse, ErrorResponse>> _getWalletQueryHandler =
    Guard.Against.Null(getWalletQueryHandler, nameof(getWalletQueryHandler));

  private readonly ICommandHandler<ContributeWalletCommand, Option<ValidationErrorResponse, ErrorResponse>> _contributeWalletCommandHandler =
    Guard.Against.Null(contributeWalletCommandHandler, nameof(contributeWalletCommandHandler));

  public async Task<OneOf<CreateWalletResponse, ValidationErrorResponse, ErrorResponse>> CreateWalletAsync(
    CreateWalletCommand command,
    CancellationToken cancellationToken = default)
  {
    Guard.Against.Null(command, nameof(command));

    return await _createWalletCommandHandler.HandleAsync(command, cancellationToken);
  }

  public async Task<OneOf<GetWalletResponse, ValidationErrorResponse, ErrorResponse>> GetWalletAsync(
    GetWalletQuery query,
    CancellationToken cancellationToken = default)
  {
    Guard.Against.Null(query, nameof(query));

    return await _getWalletQueryHandler.HandleAsync(query, cancellationToken);
  }

  public async Task<Option<ValidationErrorResponse, ErrorResponse>> ContributeWalletAsync(
    ContributeWalletCommand command,
    CancellationToken cancellationToken = default)
  {
    Guard.Against.Null(command, nameof(command));

    return await _contributeWalletCommandHandler.HandleAsync(command, cancellationToken);
  }
}
