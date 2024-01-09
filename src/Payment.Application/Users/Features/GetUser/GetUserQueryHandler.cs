using Ardalis.GuardClauses;
using FluentValidation;
using Payment.Application.Users.Repositories;
using Payment.Common.Abstraction.Models;
using Payment.Common.Abstraction.Queries;
using Payment.Domain.Entities;
using OneOf;
using Payment.Common.Mappers;
using Payment.Domain.ValueObjects;

namespace Payment.Application.Users.Features.GetUser;

public sealed class GetUserQueryHandler(
  IUserRepository userRepository,
  IValidator<GetUserQuery> getUserQueryValidator,
  IMapper<User, GetUserResponse> userToGetUserResponseMapper) :
  IQueryHandler<GetUserQuery, OneOf<GetUserResponse, ValidationErrorResponse, ErrorResponse>>
{
  private readonly IUserRepository _userRepository =
    Guard.Against.Null(userRepository, nameof(userRepository));

  private readonly IValidator<GetUserQuery> _getUserQueryValidator =
    Guard.Against.Null(getUserQueryValidator, nameof(getUserQueryValidator));

  private readonly IMapper<User, GetUserResponse> _userToGetUserResponseMapper =
    Guard.Against.Null(userToGetUserResponseMapper,
      nameof(userToGetUserResponseMapper));

  public async Task<OneOf<GetUserResponse, ValidationErrorResponse, ErrorResponse>> HandleAsync(
    GetUserQuery query,
    CancellationToken cancellationToken = default)
  {
    Guard.Against.Null(query, nameof(query));

    var validationResult = await _getUserQueryValidator.ValidateAsync(query, cancellationToken);

    if (!validationResult.IsValid)
    {
      return new ValidationErrorResponse(validationResult.Errors
        .Select(e => $"`{e.PropertyName}`: {e.ErrorMessage}")
        .ToList());
    }

    var result = await GetUserInternal(query, cancellationToken);

    return result.Match<OneOf<GetUserResponse, ValidationErrorResponse, ErrorResponse>>(
      getUserResponse => getUserResponse,
      errorResponse => errorResponse);
  }

  private async Task<OneOf<GetUserResponse, ErrorResponse>> GetUserInternal(
    GetUserQuery query,
    CancellationToken cancellationToken = default)
  {
    try
    {
      var userId = UserId.Create(Guid.Parse(query.UserId));

      var user = await _userRepository.FindByIdAsync(userId, cancellationToken);

      if (user == User.NotFound)
      {
        return new ErrorResponse { Error = $"Failed to get user with id {query.UserId}." };
      }

      return _userToGetUserResponseMapper.Map(user);
    }
    catch (Exception ex)
    {
      return new ErrorResponse { Error = $"Failed to get user with id {query.UserId} due to {ex.Message}." };
    }
  }
}
