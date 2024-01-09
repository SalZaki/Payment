using Payment.Common.Utils;

namespace Payment.Domain.Shared;

public static class CurrencyErrors
{
  public static Func<string, Error> CurrencyCodeIsInvalid => currencyCode =>
    new Error("Currency.CurrencyCodeIsInvalid", $"The given currency {currencyCode} code is not valid.");

  public static Func<int, Error> CurrencyNumberIsInvalid => currencyNumber =>
    new Error("Currency.CurrencyNumberIsInvalid", $"The given currency {currencyNumber} number is not valid.");
}
