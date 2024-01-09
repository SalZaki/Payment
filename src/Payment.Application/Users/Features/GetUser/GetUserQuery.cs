using System.Diagnostics.CodeAnalysis;
using Payment.Common.Abstraction.Models;
using Payment.Common.Abstraction.Queries;
using OneOf;

namespace Payment.Application.Users.Features.GetUser;

[ExcludeFromCodeCoverage]
public sealed record GetUserQuery : IQuery<OneOf<GetUserResponse, ValidationErrorResponse, ErrorResponse>>
{
  public required string UserId { get; init; }
}
