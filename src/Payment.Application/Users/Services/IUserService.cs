using OneOf;
using Optional;
using Payment.Application.Users.Features.GetUser;
using Payment.Application.Users.Features.AddFriendship;
using Payment.Application.Users.Features.CreateUser;
using Payment.Application.Users.Features.DeleteUser;
using Payment.Application.Users.Features.GetCommonFriends;
using Payment.Application.Users.Features.GetConnectionList;
using Payment.Common.Abstraction.Models;

namespace Payment.Application.Users.Services;

public interface IUserService
{
  Task<OneOf<CreateUserResponse, ValidationErrorResponse, ErrorResponse>> CreateUserAsync(
    CreateUserCommand command,
    CancellationToken cancellationToken = default);

  Task<OneOf<GetUserResponse, ValidationErrorResponse, ErrorResponse>> GetUserAsync(
    GetUserQuery query,
    CancellationToken cancellationToken = default);

  Task<OneOf<AddFriendshipResponse, ValidationErrorResponse, ErrorResponse>> AddFriendAsync(
    AddFriendshipCommand command,
    CancellationToken cancellationToken = default);

  Task<OneOf<IReadOnlyCollection<GetCommonFriendsResponse>, ValidationErrorResponse, ErrorResponse>> GetCommonFriendsAsync(
    GetCommonFriendsQuery query,
    CancellationToken cancellationToken = default);

  Task<Option<ValidationErrorResponse, ErrorResponse>> DeleteUserAsync(
    DeleteUserCommand command,
    CancellationToken cancellationToken = default);

  Task<OneOf<IReadOnlyCollection<GetConnectionListResponse>, ValidationErrorResponse, ErrorResponse>> GetConnectionListAsync(
    GetConnectionListQuery query,
    CancellationToken cancellationToken = default);
}
