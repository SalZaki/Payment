using Ardalis.GuardClauses;
using FluentValidation;
using OneOf;
using Payment.Application.Wallets.Repositories;
using Payment.Common.Abstraction.Models;
using Payment.Common.Abstraction.Queries;
using Payment.Common.Mappers;
using Payment.Domain.Entities;
using Payment.Domain.ValueObjects;

namespace Payment.Application.Wallets.Features.GetWallet;

public sealed class GetWalletQueryHandler(
  IWalletRepository walletRepository,
  IValidator<GetWalletQuery> getWalletQueryValidator,
  IMapper<Wallet, GetWalletResponse> walletToGetWalletResponseMapper) :
  IQueryHandler<GetWalletQuery, OneOf<GetWalletResponse, ValidationErrorResponse, ErrorResponse>>
{
  private readonly IWalletRepository _walletRepository =
    Guard.Against.Null(walletRepository, nameof(walletRepository));

  private readonly IValidator<GetWalletQuery> _getWalletQueryValidator =
    Guard.Against.Null(getWalletQueryValidator, nameof(getWalletQueryValidator));

  private readonly IMapper<Wallet, GetWalletResponse> _walletToGetWalletResponseMapper =
    Guard.Against.Null(walletToGetWalletResponseMapper,
      nameof(walletToGetWalletResponseMapper));

  public async Task<OneOf<GetWalletResponse, ValidationErrorResponse, ErrorResponse>> HandleAsync(
    GetWalletQuery query,
    CancellationToken cancellationToken = default)
  {
    Guard.Against.Null(query, nameof(query));

    var validationResult = await _getWalletQueryValidator.ValidateAsync(query, cancellationToken);

    if (!validationResult.IsValid)
    {
      return new ValidationErrorResponse(validationResult.Errors
        .Select(e => $"`{e.PropertyName}`: {e.ErrorMessage}")
        .ToList());
    }

    var result = await GetWalletInternal(query, cancellationToken);

    return result.Match<OneOf<GetWalletResponse, ValidationErrorResponse, ErrorResponse>>(
      getWalletResponse => getWalletResponse,
      errorResponse => errorResponse);
  }

  private async Task<OneOf<GetWalletResponse, ErrorResponse>> GetWalletInternal(
    GetWalletQuery query,
    CancellationToken cancellationToken = default)
  {
    try
    {
      var walletId = WalletId.Create(Guid.Parse(query.WalletId));

      var wallet = await _walletRepository.FindByIdAsync(walletId, cancellationToken);

      if (wallet == Wallet.NotFound)
      {
        return new ErrorResponse { Error = $"Failed to get wallet with id {query.WalletId}." };
      }

      return _walletToGetWalletResponseMapper.Map(wallet);
    }
    catch (Exception ex)
    {
      return new ErrorResponse { Error = $"Failed to get wallet with id {query.WalletId} due to {ex.Message}." };
    }
  }
}
