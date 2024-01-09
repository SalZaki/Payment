namespace Payment.Common.Abstraction.Domain.Exceptions;

public sealed class BusinessPolicyValidationException(string message) :
  DomainException(BusinessErrors.BusinessRuleValidationError(message));
