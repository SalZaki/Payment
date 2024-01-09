using FluentValidation;
using Payment.Application.Users.Repositories;
using Payment.Domain.Entities;
using Payment.Domain.ValueObjects;

namespace Payment.Application.Wallets.Features.ContributeWallet;

public sealed class ContributeShareCommandValidator : AbstractValidator<ContributeShareCommand>
{
  private readonly IUserRepository _userRepository;

  public ContributeShareCommandValidator(IUserRepository userRepository)
  {
    _userRepository = userRepository;

    RuleFor(x => x.ContributorId)
      .Cascade(CascadeMode.Stop)
      .NotEmpty()
      .WithErrorCode(ErrorCodes.Required(nameof(UserId)))
      .WithMessage("ContributorId is required.");

    RuleFor(x => x.ContributorId)
      .Cascade(CascadeMode.Stop)
      .Must(BeAValidGuid)
      .WithErrorCode(ErrorCodes.Invalid(nameof(UserId)))
      .WithMessage("UserId format is invalid.");

    RuleFor(x => x.ContributorId)
      .Cascade(CascadeMode.Stop)
      .MustAsync(BeAValidContributorAsync)
      .WithErrorCode(ErrorCodes.NotFound(nameof(UserId)))
      .WithMessage(x => $"Could not find a contributor with id: {x.ContributorId}");
  }

  private bool BeAValidGuid(string? id)
  {
    return id != null && Guid.TryParse(id, out _);
  }

  private async Task<bool> BeAValidContributorAsync(string contributorId, CancellationToken cancellationToken = default)
  {
    var user = await _userRepository.FindByIdAsync(UserId.Create(Guid.Parse(contributorId)), cancellationToken);

    return user != User.NotFound;
  }
}
