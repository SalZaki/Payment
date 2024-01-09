using System.Diagnostics.CodeAnalysis;
using Payment.Common.Abstraction.Models;
using Payment.Common.Abstraction.Queries;
using OneOf;

namespace Payment.Application.Users.Features.GetCommonFriends;

[ExcludeFromCodeCoverage]
public sealed record GetCommonFriendsQuery : IQuery<OneOf<IReadOnlyCollection<GetCommonFriendsResponse>, ValidationErrorResponse, ErrorResponse>>
{
  public required string UserId1 { get; init; }

  public required string UserId2 { get; init; }
}
