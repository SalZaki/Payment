using Ardalis.GuardClauses;
using FluentValidation;
using OneOf;
using Payment.Application.Users.Repositories;
using Payment.Common.Abstraction.Models;
using Payment.Common.Abstraction.Queries;
using Payment.Domain.Entities;
using Payment.Domain.ValueObjects;

namespace Payment.Application.Users.Features.GetConnectionList;

public sealed class GetConnectionListQueryHandler(
  IUserRepository userRepository,
  IValidator<GetConnectionListQuery> getConnectionListQueryValidator) :
  IQueryHandler<GetConnectionListQuery, OneOf<IReadOnlyCollection<GetConnectionListResponse>, ValidationErrorResponse, ErrorResponse>>
{

  private readonly IUserRepository _userRepository =
    Guard.Against.Null(userRepository, nameof(userRepository));

  private readonly IValidator<GetConnectionListQuery> _getConnectionListQueryValidator =
    Guard.Against.Null(getConnectionListQueryValidator, nameof(getConnectionListQueryValidator));

  public async Task<OneOf<IReadOnlyCollection<GetConnectionListResponse>, ValidationErrorResponse, ErrorResponse>>
    HandleAsync(
      GetConnectionListQuery query,
      CancellationToken cancellationToken = default)
  {
    Guard.Against.Null(query, nameof(query));

    var validationResult = await _getConnectionListQueryValidator.ValidateAsync(query, cancellationToken);

    if (!validationResult.IsValid)
    {
      return new ValidationErrorResponse(validationResult.Errors
        .Select(e => $"`{e.PropertyName}`: {e.ErrorMessage}")
        .ToList());
    }

    var result = await GetConnectionListInternal(query, cancellationToken);

    return result.Match<OneOf<IReadOnlyCollection<GetConnectionListResponse>, ValidationErrorResponse, ErrorResponse>>(
      getConnectionListResponse => getConnectionListResponse.ToList().AsReadOnly(),
      errorResponse => errorResponse);
  }

  private async Task<OneOf<IEnumerable<GetConnectionListResponse>, ErrorResponse>> GetConnectionListInternal(
    GetConnectionListQuery query,
    CancellationToken cancellationToken = default)
  {
    try
    {
      var user1Result = await GetUserAsync(query.UserId1, cancellationToken);

      if (user1Result.IsT1) return user1Result.AsT1;

      var user1 = user1Result.AsT0;

      var user2Result = await GetUserAsync(query.UserId2, cancellationToken);

      if (user2Result.IsT1) return user2Result.AsT1;

      var user2 = user2Result.AsT0;

      var result = new List<GetConnectionListResponse>();

      var connections = user1.GetConnectionList(user2, query.MaxLevel);

      if (connections.Any())
      {
        result.AddRange(connections.Select(x => new GetConnectionListResponse
        {
          UserId = x.Id.ToString(),
          FullName = x.FullName
        }));
      }

      return result;
    }
    catch (Exception ex)
    {
      return new ErrorResponse { Error = $"Failed to get connection list due to {ex.Message}." };
    }
  }

  private async Task<OneOf<User, ErrorResponse>> GetUserAsync(
    string userId,
    CancellationToken cancellationToken = default)
  {
    var user = await _userRepository.FindByIdAsync(UserId.Create(Guid.Parse(userId)), cancellationToken);

    return user == User.NotFound
      ? new ErrorResponse { Error = $"Failed to get user with id {userId}." }
      : user;
  }
}
