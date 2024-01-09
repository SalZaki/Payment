using Payment.Common.Abstraction.Domain.Exceptions;
using Payment.Domain.Shared;

namespace Payment.Domain.Exceptions;

public sealed class NegativeOrZeroAmountException(decimal amount) :
  DomainException(MoneyErrors.NegativeOrZeroAmountIsInvalid(amount));
