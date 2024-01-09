using FluentValidation;
using Payment.Application.Wallets;
using Payment.Domain.ValueObjects;

namespace Payment.Application.Users.Features.GetUser;

public sealed class GetUserQueryValidator : AbstractValidator<GetUserQuery>
{
  public GetUserQueryValidator()
  {
    RuleFor(x => x.UserId)
      .NotEmpty()
      .WithErrorCode(ErrorCodes.Required(nameof(UserId)))
      .WithMessage("UserId is required.");

    RuleFor(x => x.UserId)
      .Must(BeAValidGuid)
      .WithErrorCode(ErrorCodes.Invalid(nameof(UserId)))
      .WithMessage("UserId format is invalid.");
  }

  private bool BeAValidGuid(string? id)
  {
    return id != null && Guid.TryParse(id, out _);
  }
}
