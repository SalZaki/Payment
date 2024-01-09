using Payment.Common.Abstraction.Domain.Exceptions;
using Payment.Domain.Shared;

namespace Payment.Domain.Exceptions;

public sealed class InvalidFullNameException(string value) :
  DomainException(UserErrors.InvalidFullName(value));

public sealed class InvalidFullNameFormatException(string value) :
  DomainException(UserErrors.InvalidFullNameFormat(value));
