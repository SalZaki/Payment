using Ardalis.GuardClauses;
using FluentValidation;
using OneOf;
using Payment.Application.Wallets.Repositories;
using Payment.Common.Abstraction.Commands;
using Payment.Common.Abstraction.Models;
using Payment.Domain.Entities;
using Payment.Domain.ValueObjects;

namespace Payment.Application.Wallets.Features.CreateWallet;

public class CreateWalletCommandHandler(
  IWalletRepository walletRepository,
  IValidator<CreateWalletCommand> createWalletCommandValidator) :
  ICommandHandler<CreateWalletCommand, OneOf<CreateWalletResponse, ValidationErrorResponse, ErrorResponse>>
{
  private readonly IWalletRepository _walletRepository =
    Guard.Against.Null(walletRepository, nameof(walletRepository));

  private readonly IValidator<CreateWalletCommand> _createWalletCommandValidator =
    Guard.Against.Null(createWalletCommandValidator, nameof(createWalletCommandValidator));

  public async Task<OneOf<CreateWalletResponse, ValidationErrorResponse, ErrorResponse>> HandleAsync(
    CreateWalletCommand command,
    CancellationToken cancellationToken = default)
  {
    Guard.Against.Null(command, nameof(command));

    var validationResult = await _createWalletCommandValidator.ValidateAsync(command, cancellationToken);

    if (!validationResult.IsValid)
    {
      return new ValidationErrorResponse(validationResult.Errors
        .Select(e => $"`{e.PropertyName}`: {e.ErrorMessage}")
        .ToList());
    }

    var result = await CreateWalletInternal(command, cancellationToken);

    return result.Match<OneOf<CreateWalletResponse, ValidationErrorResponse, ErrorResponse>>(
      createWalletResponse => createWalletResponse,
      errorResponse => errorResponse);
  }

  private async Task<OneOf<CreateWalletResponse, ErrorResponse>> CreateWalletInternal(
    CreateWalletCommand command,
    CancellationToken cancellationToken = default)
  {
    try
    {
      var amount = Money.Empty();

      if (command is {Amount: not null, Currency.Length: > 0})
      {
        amount = Money.Create(command.Amount.Value, Currency.Parse(command.Currency), Units.Major);
      }

      var wallet = Wallet.Create(
        WalletId.Create(Guid.NewGuid()),
        UserId.Create(Guid.Parse(command.UserId)),
        amount,
        command.UserId,
        DateTime.UtcNow);

      await _walletRepository.AddAsync(wallet, cancellationToken);

      return new CreateWalletResponse { WalletId = wallet.Id.ToString() };
    }
    catch (Exception ex)
    {
      return new ErrorResponse { Error = $"Failed to create wallet due to {ex.Message}." };
    }
  }
}
