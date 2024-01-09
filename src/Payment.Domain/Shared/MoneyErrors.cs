using Payment.Common.Utils;

namespace Payment.Domain.Shared;

public static class MoneyErrors
{
  public static Func<decimal, Error> MaxAmountIsInvalid => amount =>
    new Error("Money.MaxAmountIsInvalid", $"The given amount {amount} is more than maximum amount allowed.");

  public static Func<decimal, Error> NegativeAmountIsInvalid => amount =>
    new Error("Money.NegativeAmountIsInvalid", $"The given amount {amount} is negative.");

  public static Func<decimal, Error> NegativeOrZeroAmountIsInvalid => amount =>
    new Error("Money.NegativeOrZeroAmountIsInvalid", $"The given amount {amount} is either negative or zero.");
}
