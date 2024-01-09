using System.Diagnostics.CodeAnalysis;
using Payment.Common.Abstraction.Commands;

namespace Payment.Application.Users.Features.CreateUser;

[ExcludeFromCodeCoverage]
public sealed record CreateUserCommand : ICommand
{
  public required string FullName { get; init; }
}
