using FluentValidation;
using Payment.Application.Users.Repositories;
using Payment.Application.Wallets;
using Payment.Domain.Entities;
using Payment.Domain.ValueObjects;

namespace Payment.Application.Users.Features.GetCommonFriends;

public sealed class GetCommonFriendsQueryValidator : AbstractValidator<GetCommonFriendsQuery>
{
  private readonly IUserRepository _userRepository;

  public GetCommonFriendsQueryValidator(IUserRepository userRepository)
  {
    _userRepository = userRepository;

    RuleFor(x => x.UserId1)
      .Cascade(CascadeMode.Stop)
      .NotEmpty()
      .WithErrorCode(ErrorCodes.Required(nameof(UserId)))
      .WithMessage("UserId1 is required.");

    RuleFor(x => x.UserId1)
      .Cascade(CascadeMode.Stop)
      .Must(BeAValidGuid)
      .WithErrorCode(ErrorCodes.Invalid(nameof(UserId)))
      .WithMessage("UserId1 format is invalid.");

    RuleFor(x => x.UserId1)
      .MustAsync(BeAValidUserAsync)
      .WithErrorCode(ErrorCodes.NotFound(nameof(UserId)))
      .WithMessage(x => $"Could not find a user with user id: {x.UserId1}");

    RuleFor(x => x.UserId2)
      .Cascade(CascadeMode.Stop)
      .NotEmpty()
      .WithErrorCode(ErrorCodes.Required(nameof(UserId)))
      .WithMessage("UserId2 is required.");

    RuleFor(x => x.UserId2)
      .Cascade(CascadeMode.Stop)
      .Must(BeAValidGuid)
      .WithErrorCode(ErrorCodes.Invalid(nameof(UserId)))
      .WithMessage("UserId2 format is invalid.");

    RuleFor(x => x.UserId2)
      .MustAsync(BeAValidUserAsync)
      .WithErrorCode(ErrorCodes.NotFound(nameof(UserId)))
      .WithMessage(x => $"Could not find a user with user id: {x.UserId2}");
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
