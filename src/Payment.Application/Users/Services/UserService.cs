using Ardalis.GuardClauses;
using OneOf;
using Optional;
using Payment.Application.Users.Features.AddFriendship;
using Payment.Application.Users.Features.CreateUser;
using Payment.Application.Users.Features.DeleteUser;
using Payment.Application.Users.Features.GetCommonFriends;
using Payment.Application.Users.Features.GetConnectionList;
using Payment.Application.Users.Features.GetUser;
using Payment.Common.Abstraction.Commands;
using Payment.Common.Abstraction.Models;
using Payment.Common.Abstraction.Queries;

namespace Payment.Application.Users.Services;

public sealed class UserService(
    IQueryHandler<GetUserQuery, OneOf<GetUserResponse, ValidationErrorResponse, ErrorResponse>> getUserQueryHandler,
    ICommandHandler<CreateUserCommand, OneOf<CreateUserResponse, ValidationErrorResponse, ErrorResponse>> createUserCommandHandler,
    ICommandHandler<AddFriendshipCommand, OneOf<AddFriendshipResponse, ValidationErrorResponse, ErrorResponse>> addFriendCommandHandler,
    IQueryHandler<GetCommonFriendsQuery, OneOf<IReadOnlyCollection<GetCommonFriendsResponse>, ValidationErrorResponse, ErrorResponse>> getCommonFriendsQueryHandler,
    ICommandHandler<DeleteUserCommand, Option<ValidationErrorResponse, ErrorResponse>> deleteUserCommandHandler,
    IQueryHandler<GetConnectionListQuery, OneOf<IReadOnlyCollection<GetConnectionListResponse>, ValidationErrorResponse, ErrorResponse>> getConnectionListHandler)
  : IUserService
{
  private readonly IQueryHandler<GetUserQuery, OneOf<GetUserResponse, ValidationErrorResponse, ErrorResponse>>
    _getUserQueryHandler = Guard.Against.Null(getUserQueryHandler, nameof(getUserQueryHandler));

  private readonly ICommandHandler<CreateUserCommand, OneOf<CreateUserResponse, ValidationErrorResponse, ErrorResponse>>
    _createUserCommandHandler = Guard.Against.Null(createUserCommandHandler, nameof(createUserCommandHandler));

  private readonly ICommandHandler<AddFriendshipCommand, OneOf<AddFriendshipResponse, ValidationErrorResponse, ErrorResponse>>
    _addFriendCommandHandler = Guard.Against.Null(addFriendCommandHandler, nameof(addFriendCommandHandler));

  private readonly IQueryHandler<GetCommonFriendsQuery, OneOf<IReadOnlyCollection<GetCommonFriendsResponse>, ValidationErrorResponse, ErrorResponse>>
    _getCommonFriendsQueryHandler = Guard.Against.Null(getCommonFriendsQueryHandler, nameof(getCommonFriendsQueryHandler));

  private readonly ICommandHandler<DeleteUserCommand, Option<ValidationErrorResponse, ErrorResponse>>
    _deleteUserCommandHandler = Guard.Against.Null(deleteUserCommandHandler, nameof(deleteUserCommandHandler));

  private readonly IQueryHandler<GetConnectionListQuery, OneOf<IReadOnlyCollection<GetConnectionListResponse>, ValidationErrorResponse, ErrorResponse>>
    _getConnectionListHandler = Guard.Against.Null(getConnectionListHandler, nameof(getConnectionListHandler));

  public async Task<OneOf<CreateUserResponse, ValidationErrorResponse, ErrorResponse>> CreateUserAsync(
    CreateUserCommand command,
    CancellationToken cancellationToken = default)
  {
    Guard.Against.Null(command, nameof(command));

    return await _createUserCommandHandler.HandleAsync(command, cancellationToken);
  }

  public async Task<OneOf<GetUserResponse, ValidationErrorResponse, ErrorResponse>> GetUserAsync(
    GetUserQuery query,
    CancellationToken cancellationToken = default)
  {
    Guard.Against.Null(query, nameof(query));

    return await _getUserQueryHandler.HandleAsync(query, cancellationToken);
  }

  public async Task<OneOf<AddFriendshipResponse, ValidationErrorResponse, ErrorResponse>> AddFriendAsync(
    AddFriendshipCommand command,
    CancellationToken cancellationToken = default)
  {
    Guard.Against.Null(command, nameof(command));

    return await _addFriendCommandHandler.HandleAsync(command, cancellationToken);
  }

  public async Task<OneOf<IReadOnlyCollection<GetCommonFriendsResponse>, ValidationErrorResponse, ErrorResponse>> GetCommonFriendsAsync(
    GetCommonFriendsQuery query,
    CancellationToken cancellationToken = default)
  {
    Guard.Against.Null(query, nameof(query));

    return await _getCommonFriendsQueryHandler.HandleAsync(query, cancellationToken);
  }

  public async Task<Option<ValidationErrorResponse, ErrorResponse>> DeleteUserAsync(
    DeleteUserCommand command,
    CancellationToken cancellationToken = default)
  {
    Guard.Against.Null(command, nameof(command));

    return await _deleteUserCommandHandler.HandleAsync(command, cancellationToken);
  }

  public async Task<OneOf<IReadOnlyCollection<GetConnectionListResponse>, ValidationErrorResponse, ErrorResponse>> GetConnectionListAsync(
    GetConnectionListQuery query,
    CancellationToken cancellationToken = default)
  {
    Guard.Against.Null(query, nameof(query));

    return await _getConnectionListHandler.HandleAsync(query, cancellationToken);
  }
}
