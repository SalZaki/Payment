using Payment.Domain.ValueObjects;

namespace Payment.Domain.UnitTests.TestData;

public static class CurrencyTestData
{
  public static readonly Currency DefaultCurrency = Currency.GBP;

  public static readonly Currency SecondaryCurrency = Currency.EUR;

  public static readonly Currency ThirdCurrency = Currency.USD;

  public static readonly Currency FourthCurrency = Currency.JPY;

  public static readonly Currency FifthCurrency = Currency.AUD;
}
