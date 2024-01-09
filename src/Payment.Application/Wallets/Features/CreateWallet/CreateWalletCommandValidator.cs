using FluentValidation;
using Payment.Application.Users.Repositories;
using Payment.Domain.Entities;
using Payment.Domain.ValueObjects;

namespace Payment.Application.Wallets.Features.CreateWallet;

public sealed class CreateWalletCommandValidator : AbstractValidator<CreateWalletCommand>
{
  private readonly IUserRepository _userRepository;

  public CreateWalletCommandValidator(IUserRepository userRepository)
  {
    _userRepository = userRepository;

    RuleFor(x => x.UserId)
      .Cascade(CascadeMode.Stop)
      .NotEmpty()
      .WithErrorCode(ErrorCodes.Required(nameof(UserId)))
      .WithMessage("OwnerId is required.");

    RuleFor(x => x.UserId)
      .Cascade(CascadeMode.Stop)
      .Must(BeAValidGuid)
      .WithErrorCode(ErrorCodes.Invalid(nameof(UserId)))
      .WithMessage("OwnerId format is invalid.");

    RuleFor(x => x.UserId)
      .Cascade(CascadeMode.Stop)
      .MustAsync(BeAValidUserAsync)
      .WithErrorCode(ErrorCodes.NotFound(nameof(UserId)))
      .WithMessage(x => $"Could not find a user with user id: {x.UserId}");
  }

  private async Task<bool> BeAValidUserAsync(string ownerId, CancellationToken cancellationToken = default)
  {
    var user = await _userRepository.FindByIdAsync(UserId.Create(Guid.Parse(ownerId)), cancellationToken);
    return user != User.NotFound;
  }

  private bool BeAValidGuid(string? id)
  {
    return id != null && Guid.TryParse(id, out _);
  }
}
