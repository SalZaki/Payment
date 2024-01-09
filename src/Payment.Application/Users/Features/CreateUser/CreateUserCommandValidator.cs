using FluentValidation;
using Payment.Application.Users.Repositories;
using Payment.Application.Wallets;
using Payment.Domain.Entities;
using Payment.Domain.ValueObjects;

namespace Payment.Application.Users.Features.CreateUser;

public sealed class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
  private readonly IUserRepository _userRepository;

  public CreateUserCommandValidator(IUserRepository userRepository)
  {
    _userRepository = userRepository;

    RuleFor(x => x.FullName)
      .Cascade(CascadeMode.Stop)
      .NotEmpty()
      .WithErrorCode(ErrorCodes.Required(nameof(FullName)))
      .WithMessage("User FullName is required.");

    RuleFor(x => x.FullName)
      .MustAsync(BeAValidUserAsync)
      .WithErrorCode(ErrorCodes.NotFound(nameof(FullName)))
      .WithMessage(x => $"User already exists with fullname: {x.FullName}");
  }

  private async Task<bool> BeAValidUserAsync(string fullname, CancellationToken cancellationToken = default)
  {
    var user = await _userRepository.FindOneAsync(x => x.FullName.Value == fullname, cancellationToken);
    return user == User.NotFound;
  }
}
