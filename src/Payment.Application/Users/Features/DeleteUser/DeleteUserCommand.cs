using System.Diagnostics.CodeAnalysis;
using Payment.Common.Abstraction.Commands;

namespace Payment.Application.Users.Features.DeleteUser;

[ExcludeFromCodeCoverage]
public sealed record DeleteUserCommand : ICommand
{
  public required string UserId { get; init; }
}
