using Ardalis.GuardClauses;
using FluentValidation;
using Optional;
using Optional.Unsafe;
using Payment.Application.Wallets.Repositories;
using Payment.Common.Abstraction.Commands;
using Payment.Common.Abstraction.Models;
using Payment.Domain.ValueObjects;

namespace Payment.Application.Wallets.Features.ContributeWallet;

public class ContributeWalletCommandHandler(
  IWalletRepository walletRepository,
  IValidator<ContributeWalletCommand> contributeWalletCommandValidator) :
  ICommandHandler<ContributeWalletCommand, Option<ValidationErrorResponse, ErrorResponse>>
{
  private readonly IWalletRepository _walletRepository =
    Guard.Against.Null(walletRepository, nameof(walletRepository));

  private readonly IValidator<ContributeWalletCommand> _contributeWalletCommandValidator =
    Guard.Against.Null(contributeWalletCommandValidator, nameof(contributeWalletCommandValidator));

  public async Task<Option<ValidationErrorResponse, ErrorResponse>> HandleAsync(
    ContributeWalletCommand command,
    CancellationToken cancellationToken = default)
  {
    Guard.Against.Null(command, nameof(command));

    var validationResult = await _contributeWalletCommandValidator.ValidateAsync(command, cancellationToken);

    if (!validationResult.IsValid)
    {
      return Option.Some<ValidationErrorResponse, ErrorResponse>(new ValidationErrorResponse(validationResult.Errors
        .Select(e => $"{e.PropertyName}: {e.ErrorMessage}")
        .ToList()));
    }

    var result = await ContributeWalletInternal(command, cancellationToken);

    return result.HasValue
      ? Option.None<ValidationErrorResponse, ErrorResponse>(result.ValueOrFailure())
      : new Option<ValidationErrorResponse, ErrorResponse>();
  }

  private async Task<Option<ErrorResponse>> ContributeWalletInternal(
    ContributeWalletCommand command,
    CancellationToken cancellationToken = default)
  {
    try
    {
      var walletId = WalletId.Create(Guid.Parse(command.WalletId));
      var wallet = await _walletRepository.FindByIdAsync(walletId, cancellationToken);

      foreach (var share in command.Shares)
      {
        var amount = Money.Create(share.Amount, Currency.Parse(share.Currency), Units.Major);
        var contributorId = UserId.Create(Guid.Parse(share.ContributorId));
        wallet.Contribute(amount, contributorId, command.CreatedOrModifiedBy, command.CreatedOrModifiedOn);
      }

      await _walletRepository.UpdateAsync(wallet, cancellationToken);

      return Option.None<ErrorResponse>();
    }
    catch (Exception ex)
    {
      return Option.Some(new ErrorResponse {Error = $"Failed to contribute wallet with id {command.WalletId} due to {ex.Message}."});
    }
  }
}
