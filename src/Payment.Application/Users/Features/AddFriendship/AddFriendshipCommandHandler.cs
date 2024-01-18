using Ardalis.GuardClauses;
using FluentValidation;
using OneOf;
using Optional;
using Optional.Unsafe;
using Payment.Application.Users.Repositories;
using Payment.Common.Abstraction.Commands;
using Payment.Common.Abstraction.Models;
using Payment.Domain.Entities;
using Payment.Domain.ValueObjects;

namespace Payment.Application.Users.Features.AddFriendship;

public class AddFriendshipCommandHandler(
  IUserRepository userRepository,
  IValidator<AddFriendshipCommand> addFriendshipCommandValidator) :
  ICommandHandler<AddFriendshipCommand, OneOf<AddFriendshipResponse, ValidationErrorResponse, ErrorResponse>>
{
  private readonly IUserRepository _userRepository =
    Guard.Against.Null(userRepository, nameof(userRepository));

  private readonly IValidator<AddFriendshipCommand> _addFriendshipCommandValidator =
    Guard.Against.Null(addFriendshipCommandValidator, nameof(addFriendshipCommandValidator));

  public async Task<OneOf<AddFriendshipResponse, ValidationErrorResponse, ErrorResponse>> HandleAsync(
    AddFriendshipCommand command,
    CancellationToken cancellationToken = default)
  {
    Guard.Against.Null(command, nameof(command));

    var validationResult = await _addFriendshipCommandValidator.ValidateAsync(command, cancellationToken);

    if (!validationResult.IsValid)
    {
      return new ValidationErrorResponse(validationResult.Errors
        .Select(e => $"`{e.PropertyName}`: {e.ErrorMessage}")
        .ToList());
    }

    var result = await AddFriendshipInternal(command, cancellationToken);

    return result.Match<OneOf<AddFriendshipResponse, ValidationErrorResponse, ErrorResponse>>(
      createUserResponse => createUserResponse,
      errorResponse => errorResponse);
  }

  private async Task<OneOf<AddFriendshipResponse, ErrorResponse>> AddFriendshipInternal(
    AddFriendshipCommand command,
    CancellationToken cancellationToken = default)
  {
    try
    {
      var userResult = await GetUserAsync(Guid.Parse(command.UserId), cancellationToken);

      if (userResult.IsT1) return userResult.AsT1;

      var user = userResult.AsT0;

      var friendResult = await GetUserAsync(Guid.Parse(command.FriendId), cancellationToken);

      if (friendResult.IsT1) return friendResult.AsT1;

      var friend = friendResult.AsT0;

      var result = await AddFriendAsync(user, friend, cancellationToken);

      if (result.HasValue) return result.ValueOrDefault();

      return new AddFriendshipResponse { FriendId = friend.Id.ToString() };
    }
    catch (Exception ex)
    {
      return new ErrorResponse {Error = $"Failed to add friend due to {ex.Message}."};
    }
  }

  private async Task<OneOf<User, ErrorResponse>> GetUserAsync(
    Guid userId,
    CancellationToken cancellationToken = default)
  {
    var user = await _userRepository.FindByIdAsync(UserId.Create(userId), cancellationToken);

    return user == User.NotFound
      ? new ErrorResponse {Error = $"Failed to get user with id {userId}."}
      : user;
  }

  private async Task<Option<ErrorResponse>> AddFriendAsync(
    User user,
    User friend,
    CancellationToken cancellationToken = default)
  {
    try
    {
      user.AddFriend(friend);
      await _userRepository.UpdateAsync(user, cancellationToken);

      friend.AddFriend(user);
      await _userRepository.UpdateAsync(friend, cancellationToken);

      return Option.None<ErrorResponse>();
    }
    catch (Exception ex)
    {
      return Option.Some(new ErrorResponse {Error = $"Failed to add friend due to {ex.Message}."});
    }
  }
}
