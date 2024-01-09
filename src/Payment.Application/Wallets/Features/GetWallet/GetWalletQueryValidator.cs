using FluentValidation;
using Payment.Domain.ValueObjects;

namespace Payment.Application.Wallets.Features.GetWallet;

public sealed class GetWalletQueryValidator : AbstractValidator<GetWalletQuery>
{
  public GetWalletQueryValidator()
  {
    RuleFor(x => x.WalletId)
      .NotEmpty()
      .WithErrorCode(ErrorCodes.Required(nameof(WalletId)))
      .WithMessage("WalletId is required.");

    RuleFor(x => x.WalletId)
      .Must(BeAValidGuid)
      .WithErrorCode(ErrorCodes.Invalid(nameof(WalletId)))
      .WithMessage("WalletId format is invalid.");
  }

  private bool BeAValidGuid(string? id)
  {
    return id != null && Guid.TryParse(id, out _);
  }
}
