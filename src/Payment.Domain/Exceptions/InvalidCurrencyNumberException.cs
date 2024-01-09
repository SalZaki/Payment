using Payment.Common.Abstraction.Domain.Exceptions;
using Payment.Domain.Shared;

namespace Payment.Domain.Exceptions;

public sealed class InvalidCurrencyNumberException(int currencyNumber) :
  DomainException(CurrencyErrors.CurrencyNumberIsInvalid(currencyNumber));
