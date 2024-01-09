using Payment.Common.Abstraction.Domain.Exceptions;
using Payment.Domain.Shared;

namespace Payment.Domain.Exceptions;

public sealed class FriendAlreadyExistsException(string id) :
  DomainException(UserErrors.FriendAlreadyExists(id));

