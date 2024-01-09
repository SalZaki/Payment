using FluentValidation;
using Payment.Application.Wallets.Repositories;
using Payment.Domain.Entities;
using Payment.Domain.ValueObjects;

namespace Payment.Application.Wallets.Features.ContributeWallet;

public sealed class ContributeWalletCommandValidator : AbstractValidator<ContributeWalletCommand>
{
  private readonly IWalletRepository _walletRepository;

  public ContributeWalletCommandValidator(IWalletRepository walletRepository)
  {
    _walletRepository = walletRepository;

    RuleFor(x => x.WalletId)
      .Cascade(CascadeMode.Stop)
      .NotEmpty()
      .WithErrorCode(ErrorCodes.Required(nameof(WalletId)))
      .WithMessage("WalletId is required.");

    RuleFor(x => x.WalletId)
      .Cascade(CascadeMode.Stop)
      .Must(BeAValidGuid)
      .WithErrorCode(ErrorCodes.Invalid(nameof(WalletId)))
      .WithMessage("WalletId format is invalid.");

    RuleFor(x => x.WalletId)
      .Cascade(CascadeMode.Stop)
      .MustAsync(BeAValidWalletAsync)
      .WithErrorCode(ErrorCodes.NotFound(nameof(WalletId)))
      .WithMessage(x => $"Could not find a wallet with id: {x.WalletId}");
  }

  private async Task<bool> BeAValidWalletAsync(string walletId, CancellationToken cancellationToken = default)
  {
    var wallet = await _walletRepository.FindByIdAsync(WalletId.Create(Guid.Parse(walletId)), cancellationToken);

    return wallet != Wallet.NotFound;
  }

  private bool BeAValidGuid(string? id)
  {
    return id != null && Guid.TryParse(id, out _);
  }
}

