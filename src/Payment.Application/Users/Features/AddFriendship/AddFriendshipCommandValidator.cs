using FluentValidation;
using Payment.Application.Users.Repositories;
using Payment.Application.Wallets;
using Payment.Domain.Entities;
using Payment.Domain.ValueObjects;

namespace Payment.Application.Users.Features.AddFriendship;

public sealed class AddFriendshipCommandValidator : AbstractValidator<AddFriendshipCommand>
{
  private readonly IUserRepository _userRepository;

  public AddFriendshipCommandValidator(IUserRepository userRepository)
  {
    _userRepository = userRepository;

    RuleFor(x => x.UserId)
      .Cascade(CascadeMode.Stop)
      .NotEmpty()
      .WithErrorCode(ErrorCodes.Required(nameof(UserId)))
      .WithMessage("UserId is required.");

    RuleFor(x => x.UserId)
      .Cascade(CascadeMode.Stop)
      .Must(BeAValidGuid)
      .WithErrorCode(ErrorCodes.Invalid(nameof(UserId)))
      .WithMessage("UserId format is invalid.");

    RuleFor(x => x.UserId)
      .MustAsync(BeAValidUserAsync)
      .WithErrorCode(ErrorCodes.NotFound(nameof(UserId)))
      .WithMessage(x => $"Could not find a user with user id: {x.UserId}");

    RuleFor(x => x.FriendId)
      .Cascade(CascadeMode.Stop)
      .NotEmpty()
      .WithErrorCode(ErrorCodes.Required(nameof(FriendId)))
      .WithMessage("FriendId is required.");

    RuleFor(x => x.FriendId)
      .Cascade(CascadeMode.Stop)
      .Must(BeAValidGuid)
      .WithErrorCode(ErrorCodes.Invalid(nameof(FriendId)))
      .WithMessage("FriendId format is invalid.");

    RuleFor(x => x.FriendId)
      .MustAsync(BeAValidUserAsync)
      .WithErrorCode(ErrorCodes.NotFound(nameof(UserId)))
      .WithMessage(x => $"Could not find a friend with user id: {x.FriendId}");
  }

  private async Task<bool> BeAValidUserAsync(string id, CancellationToken cancellationToken = default)
  {
    var user = await _userRepository.FindByIdAsync(UserId.Create(Guid.Parse(id)), cancellationToken);
    return user != User.NotFound;
  }

  private bool BeAValidGuid(string? id)
  {
    return id != null && Guid.TryParse(id, out _);
  }
}
