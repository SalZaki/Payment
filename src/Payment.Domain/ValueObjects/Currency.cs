using Ardalis.GuardClauses;
using Payment.Domain.Exceptions;

namespace Payment.Domain.ValueObjects;

public partial record Currency : IComparable<Currency>, IComparable
{
  private int _conversionFactor;

  public required string Name { get; init; }

  public required string Code { get; init; }

  public required int? Number{ get; init; }

  public required int? MinorUnits{ get; init; }

  public required HashSet<string> Countries;

  public static Currency Create(string name, string code, int? number, int? minorUnits, string[] countries)
  {
    Guard.Against.NullOrEmpty(name, nameof(name), "Currency name can not be null or empty.");
    Guard.Against.NullOrEmpty(code, nameof(code), "Currency code can not be null or empty.");
    Guard.Against.Null(countries, nameof(countries), "Units can not be null.");

    return new Currency
    {
      Name = name,
      Code = code,
      Number = number,
      MinorUnits = minorUnits,
      Countries = new HashSet<string>(countries),
      _conversionFactor = Convert.ToInt32(Math.Pow(10, minorUnits ?? 0))
    };
  }

  public decimal ToValueInMajorUnits(decimal valueInMinorUnits)
  {
    return valueInMinorUnits / _conversionFactor;
  }

  public decimal ToValueInMinorUnits(decimal valueInMajorUnits)
  {
    return valueInMajorUnits * _conversionFactor;
  }

  public override string ToString()
  {
    return $"Code: {Code} Name: {Name} Number: {Number} Country(s): [{string.Join(",", Countries.Select(x => x))}]";
  }

  public int CompareTo(Currency? other)
  {
    return other is null ? 1 : string.Compare(Code, other.Code, StringComparison.Ordinal);
  }

  public int CompareTo(object? obj)
  {
    if (obj is null) return 1;

    return obj is not Currency currency ? 1 : CompareTo(currency);
  }

  public static Currency Parse(string currencyCode)
  {
    Guard.Against.NullOrWhiteSpace(currencyCode, nameof(currencyCode), "Currency code can not be null or empty.");

    if (CurrencyCodeLookup.TryGetValue(currencyCode, out var currency)) return currency;

    throw new InvalidCurrencyCodeException(currencyCode);
  }

  public static Currency Parse(int currencyNumber)
  {
    Guard.Against.NegativeOrZero(currencyNumber, nameof(currencyNumber), "Currency number can not be zero or negative.");

    if (CurrencyNumberLookup.TryGetValue(currencyNumber, out var currency)) return currency;

    throw new InvalidCurrencyNumberException(currencyNumber);
  }
}

public sealed class InvariantCultureIgnoreCaseTrimmedEqualityStringComparer : IEqualityComparer<string>
{
  public bool Equals(string? x, string? y) => string.Equals(x?.Trim(), y?.Trim(), StringComparison.InvariantCultureIgnoreCase);

  public int GetHashCode(string obj) => obj.Trim().ToUpperInvariant().GetHashCode();
}
