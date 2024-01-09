using Payment.Common.Utils;

namespace Payment.Common.Abstraction.Domain.Exceptions;

public static class BusinessErrors
{
  public static Func<string, Error> BusinessRuleValidationError => message =>
    new Error("Business.BusinessRuleValidationError", message);
}
