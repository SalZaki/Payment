using Payment.Common.Abstraction.Domain.Exceptions;
using Payment.Domain.Shared;

namespace Payment.Domain.Exceptions;

public sealed class MaxAmountException(decimal amount) :
  DomainException(MoneyErrors.MaxAmountIsInvalid(amount));
