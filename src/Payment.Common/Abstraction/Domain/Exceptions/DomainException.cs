using Payment.Common.Utils;

namespace Payment.Common.Abstraction.Domain.Exceptions;

public abstract class DomainException(Error error) : Exception(error.Message);
