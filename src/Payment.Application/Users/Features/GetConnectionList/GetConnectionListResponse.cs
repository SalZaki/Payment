using System.Diagnostics.CodeAnalysis;

namespace Payment.Application.Users.Features.GetConnectionList;

[ExcludeFromCodeCoverage]
public class GetConnectionListResponse
{
  public required string UserId { get; init; }

  public required string FullName { get; init; }
}
