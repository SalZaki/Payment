using Ardalis.GuardClauses;
using FluentValidation;
using Payment.Application.Users.Repositories;
using Payment.Common.Abstraction.Commands;
using Payment.Common.Abstraction.Models;
using Payment.Domain.Entities;
using Payment.Domain.ValueObjects;
using OneOf;

namespace Payment.Application.Users.Features.CreateUser;

public class CreateUserCommandHandler(
  IUserRepository userRepository,
  IValidator<CreateUserCommand> createUserCommandValidator) :
  ICommandHandler<CreateUserCommand, OneOf<CreateUserResponse, ValidationErrorResponse, ErrorResponse>>
{
  private readonly IUserRepository _userRepository =
    Guard.Against.Null(userRepository, nameof(userRepository));

  private readonly IValidator<CreateUserCommand> _createUserCommandValidator =
    Guard.Against.Null(createUserCommandValidator, nameof(createUserCommandValidator));

  public async Task<OneOf<CreateUserResponse, ValidationErrorResponse, ErrorResponse>> HandleAsync(
    CreateUserCommand command,
    CancellationToken cancellationToken = default)
  {
    Guard.Against.Null(command, nameof(command));

    var validationResult = await _createUserCommandValidator.ValidateAsync(command, cancellationToken);

    if (!validationResult.IsValid)
    {
      return new ValidationErrorResponse(validationResult.Errors
        .Select(e => $"{e.PropertyName}: {e.ErrorMessage}")
        .ToList());
    }

    var result = await CreateUserInternal(command, cancellationToken);

    return result.Match<OneOf<CreateUserResponse, ValidationErrorResponse, ErrorResponse>>(
      createUserResponse => createUserResponse,
      errorResponse => errorResponse);
  }

  private async Task<OneOf<CreateUserResponse, ErrorResponse>> CreateUserInternal(
    CreateUserCommand command,
    CancellationToken cancellationToken = default)
  {
    try
    {
      var user = User.Create(
        UserId.Create(Guid.NewGuid()),
        FullName.Create(command.FullName),
        command.FullName,
        DateTime.UtcNow);

      await _userRepository.AddAsync(user, cancellationToken);

      return new CreateUserResponse { UserId = user.Id.ToString() };
    }
    catch (Exception ex)
    {
      return new ErrorResponse { Error = $"Failed to create user due to {ex.Message}." };
    }
  }
}
