using Ardalis.GuardClauses;
using Payment.Domain.Exceptions;
using Payment.Domain.Extensions;

namespace Payment.Domain.ValueObjects;

/// <summary>
/// Represents a money value, i.e. a decimal amount in a currency.
/// Allows clear understanding of the value in major or minor units.
/// <example> Major Units (e.g. 100 USD meaning 100 dollars or 10,000 cents) </example>
/// <example> Minor Units (e.g. 100 USD meaning 100 cents or 1 dollar) </example>
/// </summary>
public sealed record Money : IComparable<Money>, IComparable
{
  public required Currency Currency { get; init; }

  public required Units Units { get; init; }

  /// <summary>
  /// The money value in the major units of a given currency
  /// <example>
  /// If Currency is GBP, 125.12 is one hundred and twenty five British pounds sterling and twelve pence
  /// If Currency is JPY, 125 is one hundred and twenty five Japanese Yen
  /// </example>
  /// </summary>
  public decimal InMajorUnits { get; private set; }

  /// <summary>
  /// The money value in the minor units of a given currency
  /// <example>
  /// If Currency is GBP, 125.12 is one British pounds sterling and 25.12 pence
  /// If Currency is JPY, 125 is one hundred and twenty five Japanese Yen (JPY has no sub units, so the major and minor unit values are the same)
  /// </example>
  /// </summary>
  public decimal InMinorUnits { get; private set; }

  public static Money Empty()
  {
    return new Money
    {
      Currency = null!,
      Units = Units.Major
    };
  }

  public static Money Create(decimal amount, Currency currency, Units units)
  {
    Guard.Against.Null(currency, nameof(currency), "Currency can not be null.");
    Guard.Against.Null(units, nameof(units), "Units can not be null.");

    var money = new Money {Currency = currency, Units = units};

     money.SetAmount(amount);

    return money;
  }

  public void SetAmount(decimal amount)
  {
    // We are assuming amount can not be negative or more than 100,000 in any currency.
    Guard.Against.Negative(amount);
    Guard.Against.MaxAmount(amount);

    if (Units.Value == Units.Major.Value)
    {
      InMajorUnits = amount;
      InMinorUnits = Currency.ToValueInMinorUnits(amount);
    }

    if (Units.Value == Units.Minor.Value)
    {
      InMinorUnits = amount;
      InMajorUnits = Currency.ToValueInMajorUnits(amount);
    }

    // Should raise domain event here called "AmountSet"
  }

  public void UpdateAmount(decimal amount)
  {
    // We are assuming amount can not be negative, zero or more than 100,000 in any currency.
    Guard.Against.NegativeOrZero(amount);
    Guard.Against.MaxAmount(amount);

    if (Units.Value == Units.Major.Value)
    {
      InMajorUnits += amount;
      InMinorUnits += Currency.ToValueInMinorUnits(amount);
    }

    if (Units.Value == Units.Minor.Value)
    {
      InMinorUnits += amount;
      InMajorUnits += Currency.ToValueInMajorUnits(amount);
    }

    // Should raise domain event here called "AmountUpdated"
  }

  public override string ToString()
  {
    return $"Currency: {Currency.Code} Major: {InMajorUnits} Minor: {InMinorUnits})";
  }

  public int CompareTo(Money? other)
  {
    if (other is null) return 1;

    var result = string.Compare(Currency.Code, other.Currency.Code, StringComparison.Ordinal);

    return result != 0 ? result : InMajorUnits.CompareTo(other.InMajorUnits);
  }

  public int CompareTo(object? obj)
  {
    if (obj is null) return 1;

    return obj is not Money money ? 1 : CompareTo(money);
  }

  public static bool operator >(Money a, Money b)
  {
    ValidateCurrencyMatchOrThrow(a, b);

    return a.InMajorUnits > b.InMajorUnits;
  }

  public static bool operator <(Money a, Money b)
  {
    ValidateCurrencyMatchOrThrow(a, b);

    return a.InMajorUnits < b.InMajorUnits;
  }

  public static bool operator >=(Money a, Money b)
  {
    ValidateCurrencyMatchOrThrow(a, b);

    return a.InMajorUnits >= b.InMajorUnits;
  }

  public static bool operator <=(Money a, Money b)
  {
    ValidateCurrencyMatchOrThrow(a, b);

    return a.InMajorUnits <= b.InMajorUnits;
  }

  public static Money operator +(Money a, Money b)
  {
    ValidateCurrencyMatchOrThrow(a, b);

    return Create(a.InMajorUnits + b.InMajorUnits, a.Currency, Units.Major);
  }

  public static Money operator -(Money a, Money b)
  {
    ValidateCurrencyMatchOrThrow(a, b);

    return Create(a.InMajorUnits - b.InMajorUnits, a.Currency, Units.Major);
  }

  private static bool HasSameCurrency(Money a, Money b)
  {
    return a.Currency == b.Currency;
  }

  private static void ValidateCurrencyMatchOrThrow(Money a, Money b)
  {
    if (!HasSameCurrency(a, b)) throw new CurrencyMismatchException();
  }
}
