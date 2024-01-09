using Payment.Common.Abstraction.Domain;
using Payment.Domain.ValueObjects;

namespace Payment.Domain.Policies;

public sealed record UserCannotContributeToOwnWallet(UserId UserId, UserId ContributorId) : IBusinessPolicy
{
  public string Message => $"User with id {UserId} cannot contribute to its own wallet";

  public bool IsInvalid()
  {
    return UserId == ContributorId;
  }
}
