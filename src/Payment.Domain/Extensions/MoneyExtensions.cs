using Payment.Domain.ValueObjects;

namespace Payment.Domain.Extensions;

public static class MoneyExtensions
{
  public static IDictionary<string, decimal> SumCurrencies(this IEnumerable<Money> monies)
  {
    if (monies == null)
    {
      throw new ArgumentOutOfRangeException(nameof(monies), "One or more Money object must be specified");
    }

    return monies
      .GroupBy(x => x.Currency.Code)
      .ToDictionary(
        x => x.Key,
        x => x.Sum(y => y.InMajorUnits));
  }
}
