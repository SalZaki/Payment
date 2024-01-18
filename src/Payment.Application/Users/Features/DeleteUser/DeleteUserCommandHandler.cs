using Ardalis.GuardClauses;
using FluentValidation;
using Optional;
using Optional.Unsafe;
using Payment.Application.Users.Repositories;
using Payment.Common.Abstraction.Commands;
using Payment.Common.Abstraction.Models;
using Payment.Domain.ValueObjects;

namespace Payment.Application.Users.Features.DeleteUser;

public class DeleteUserCommandHandler(
  IUserRepository userRepository,
  IValidator<DeleteUserCommand> deleteUserCommandValidator) :
  ICommandHandler<DeleteUserCommand, Option<ValidationErrorResponse, ErrorResponse>>
{
  private readonly IUserRepository _userRepository =
    Guard.Against.Null(userRepository, nameof(userRepository));

  private readonly IValidator<DeleteUserCommand> _deleteUserCommandValidator =
    Guard.Against.Null(deleteUserCommandValidator, nameof(deleteUserCommandValidator));

  public async Task<Option<ValidationErrorResponse, ErrorResponse>> HandleAsync(
    DeleteUserCommand command,
    CancellationToken cancellationToken = default)
  {
    Guard.Against.Null(command, nameof(command));

    var validationResult = await _deleteUserCommandValidator.ValidateAsync(command, cancellationToken);

    if (!validationResult.IsValid)
    {
      return Option.Some<ValidationErrorResponse, ErrorResponse>(new ValidationErrorResponse(validationResult.Errors
        .Select(e => $"`{e.PropertyName}`: {e.ErrorMessage}")
        .ToList()));
    }

    var result = await DeleteUserInternal(command, cancellationToken);

    return result.HasValue
      ? Option.None<ValidationErrorResponse, ErrorResponse>(result.ValueOrFailure())
      : new Option<ValidationErrorResponse, ErrorResponse>();
  }

  private async Task<Option<ErrorResponse>> DeleteUserInternal(
    DeleteUserCommand command,
    CancellationToken cancellationToken = default)
  {
    try
    {
      var userId = UserId.Create(Guid.Parse(command.UserId));

      var user = await _userRepository.FindByIdAsync(userId, cancellationToken);

      var friends = user.Friendships.Select(x => x.Friend);

      foreach (var friend in friends)
      {
        friend.RemoveFriend(user);
        await _userRepository.UpdateAsync(friend, cancellationToken);
      }

      await _userRepository.DeleteByIdAsync(userId, cancellationToken);

      return Option.None<ErrorResponse>();
    }
    catch (Exception ex)
    {
      return Option.Some(new ErrorResponse {Error = $"Failed to delete user due to {ex.Message}."});
    }
  }
}
