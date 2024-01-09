using OneOf;
using System.Diagnostics.CodeAnalysis;
using Payment.Common.Abstraction.Models;
using Payment.Common.Abstraction.Queries;

namespace Payment.Application.Users.Features.GetConnectionList;

[ExcludeFromCodeCoverage]
public class GetConnectionListQuery : IQuery<OneOf<IReadOnlyCollection<GetConnectionListResponse>, ValidationErrorResponse, ErrorResponse>>
{
  public required string UserId1 { get; init; }

  public required string UserId2 { get; init; }

  public int MaxLevel { get; init; } = 100;
}
