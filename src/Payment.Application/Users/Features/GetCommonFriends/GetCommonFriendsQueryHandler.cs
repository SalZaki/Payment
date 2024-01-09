using Ardalis.GuardClauses;
using FluentValidation;
using OneOf;
using Payment.Application.Users.Repositories;
using Payment.Common.Abstraction.Models;
using Payment.Common.Abstraction.Queries;
using Payment.Domain.Entities;
using Payment.Domain.ValueObjects;

namespace Payment.Application.Users.Features.GetCommonFriends;

public sealed class GetCommonFriendsQueryHandler(
  IUserRepository userRepository,
  IValidator<GetCommonFriendsQuery> getCommonFriendsQueryValidator) :
  IQueryHandler<GetCommonFriendsQuery, OneOf<IReadOnlyCollection<GetCommonFriendsResponse>, ValidationErrorResponse, ErrorResponse>>
{
  private readonly IUserRepository _userRepository =
    Guard.Against.Null(userRepository, nameof(userRepository));

  private readonly IValidator<GetCommonFriendsQuery> _getCommonFriendsQueryValidator =
    Guard.Against.Null(getCommonFriendsQueryValidator, nameof(getCommonFriendsQueryValidator));

  public async Task<OneOf<IReadOnlyCollection<GetCommonFriendsResponse>, ValidationErrorResponse, ErrorResponse>> HandleAsync(
    GetCommonFriendsQuery query,
    CancellationToken cancellationToken = default)
  {
    Guard.Against.Null(query, nameof(query));

    var validationResult = await _getCommonFriendsQueryValidator.ValidateAsync(query, cancellationToken);

    if (!validationResult.IsValid)
    {
      return new ValidationErrorResponse(validationResult.Errors
        .Select(e => $"`{e.PropertyName}`: {e.ErrorMessage}")
        .ToList());
    }

    var result = await GetCommonFriendsInternal(query, cancellationToken);

    return result.Match<OneOf<IReadOnlyCollection<GetCommonFriendsResponse>, ValidationErrorResponse, ErrorResponse>>(
      getCommonFriendsResponse => getCommonFriendsResponse.ToList().AsReadOnly(),
      errorResponse => errorResponse);
  }

  private async Task<OneOf<IEnumerable<GetCommonFriendsResponse>, ErrorResponse>> GetCommonFriendsInternal(
    GetCommonFriendsQuery query,
    CancellationToken cancellationToken = default)
  {
    try
    {
      var user1Result = await GetUserAsync(Guid.Parse(query.UserId1), cancellationToken);
      if (user1Result.IsT1) return user1Result.AsT1;
      var user1 = user1Result.AsT0;

      var user2Result = await GetUserAsync(Guid.Parse(query.UserId2), cancellationToken);
      if (user2Result.IsT1) return user2Result.AsT1;
      var user2 = user1Result.AsT0;

      var result = new List<GetCommonFriendsResponse>();

      var commonFriends = user1.GetCommonFriends(user2);

      if (commonFriends.Any())
      {
        result.AddRange(commonFriends.Select(x => new GetCommonFriendsResponse
        {
          UserId = x.Id.ToString(),
          FullName = x.FullName
        }));
      }

      return result;
    }
    catch (Exception ex)
    {
      return new ErrorResponse { Error = $"Failed to get common friends due to {ex.Message}." };
    }
  }

  private async Task<OneOf<User, ErrorResponse>> GetUserAsync(
    Guid userId,
    CancellationToken cancellationToken = default)
  {
    var user = await _userRepository.FindByIdAsync(UserId.Create(userId), cancellationToken);

    return user == User.NotFound
      ? new ErrorResponse { Error = $"Failed to get user with id {userId}." }
      : user;
  }
}
