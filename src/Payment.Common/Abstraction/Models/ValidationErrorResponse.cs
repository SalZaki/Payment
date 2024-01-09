using System.Diagnostics.CodeAnalysis;

namespace Payment.Common.Abstraction.Models;

[ExcludeFromCodeCoverage]
public sealed record ValidationErrorResponse
{
  public IReadOnlyCollection<string> Errors { get; }

  public ValidationErrorResponse(IEnumerable<string> errors)
  {
    Errors = errors.ToList();
  }
}
