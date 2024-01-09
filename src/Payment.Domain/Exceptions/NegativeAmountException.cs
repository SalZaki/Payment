using Payment.Common.Abstraction.Domain.Exceptions;
using Payment.Domain.Shared;

namespace Payment.Domain.Exceptions;

public sealed class NegativeAmountException(decimal amount) :
  DomainException(MoneyErrors.NegativeAmountIsInvalid(amount));
