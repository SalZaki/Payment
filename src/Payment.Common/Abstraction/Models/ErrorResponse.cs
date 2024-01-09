using System.Diagnostics.CodeAnalysis;

namespace Payment.Common.Abstraction.Models;

[ExcludeFromCodeCoverage]
public sealed record ErrorResponse
{
  public required string Error { get; init; }
}
