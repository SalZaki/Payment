using AutoFixture.Idioms;
using Payment.Domain.ValueObjects;
using FluentAssertions;
using Payment.Domain.Exceptions;
using Xunit;

namespace Payment.Domain.UnitTests.ValueObjects;

/// <summary>
/// Could filter by `dotnet test --filter "Class=Currency"` in running tests in command line
/// </summary>
[Trait("Class", "Currency")]
public sealed class CurrencyTests
{
  [Theory]
  [Trait("Method", "Currency.Constructor")]
  [MoneyAutoNSubstituteData]
  public void should_have_constructor_guard_clauses(GuardClauseAssertion assertion)
  {
    // act & assert
    assertion.Verify(typeof(Currency).GetConstructors());
  }

  [Theory]
  [Trait("Method", "Currency.Parse")]
  [InlineData("Euro", "EUR", 978, 2, "ITALY")]
  [InlineData("Algerian Dinar", "DZD", 12, 2, "ALGERIA")]
  [InlineData("Pound Sterling", "GBP", 826, 2, "ISLE OF MAN")]
  public void should_parse_currency_with_currency_code(
    string name,
    string code,
    int number,
    int minorUnits,
    string country)
  {
    // act
    var currency = Currency.Parse(code);

    // assert
    currency.Should().NotBeNull();
    currency.Name.Should().Be(name);
    currency.Code.Should().Be(code);
    currency.Number.Should().Be(number);
    currency.MinorUnits.Should().Be(minorUnits);
    currency.Countries.Should().NotBeNull();
    currency.Countries.Contains(country).Should().BeTrue();
  }

  [Theory]
  [Trait("Method", "Currency.Parse")]
  [InlineData("Euro", "EUR", 978, 2, "ITALY")]
  [InlineData("Vatu", "VUV", 548, 0, "VANUATU")]
  [InlineData("Pound Sterling", "GBP", 826, 2, "ISLE OF MAN")]
  public void should_parse_currency_with_currency_number(
    string name,
    string code,
    int number,
    int minorUnits,
    string country)
  {
    // act
    var currency = Currency.Parse(number);

    // assert
    currency.Should().NotBeNull();
    currency.Name.Should().Be(name);
    currency.Code.Should().Be(code);
    currency.Number.Should().Be(number);
    currency.MinorUnits.Should().Be(minorUnits);
    currency.Countries.Should().NotBeNull();
    currency.Countries.Contains(country).Should().BeTrue();
  }

  [Theory]
  [Trait("Method", "Currency.Parse")]
  [InlineData("")]
  [InlineData(" ")]
  public void should_throw_exception_when_parsing_given_null_or_empty_code(string currencyCode)
  {
    // act
    var act = () => { Currency.Parse(currencyCode); };

    // assert
    act.Should()
      .Throw<ArgumentException>()
      .Where(x => x.Message.StartsWith("Currency code can not be null or empty."))
      .WithParameterName(nameof(currencyCode));
  }

  [Theory]
  [Trait("Method", "Currency.Parse")]
  [InlineData("eec32432cj")]
  [InlineData("SWQOD23")]
  public void should_throw_exception_when_parsing_given_invalid_code(string currencyCode)
  {
    // act
    var act = () => { Currency.Parse(currencyCode); };

    // assert
    act.Should()
      .Throw<InvalidCurrencyCodeException>()
      .WithMessage($"The given currency {currencyCode} code is not valid.");
  }

  [Theory]
  [Trait("Method", "Currency.Parse")]
  [InlineData(0)]
  [InlineData(-34)]
  [InlineData(int.MinValue)]
  public void should_throw_exception_when_parsing_given_zero_or_negative_number(int currencyNumber)
  {
    // act
    var act = () => { Currency.Parse(currencyNumber); };

    // assert
    act.Should()
      .Throw<ArgumentException>()
      .Where(x => x.Message.StartsWith("Currency number can not be zero or negative."))
      .WithParameterName(nameof(currencyNumber));
  }

  [Theory]
  [Trait("Method", "Currency.Parse")]
  [InlineData(2330)]
  [InlineData(int.MaxValue)]
  public void should_throw_exception_when_parsing_given_invalid_number(int currencyNumber)
  {
    // act
    var act = () => { Currency.Parse(currencyNumber); };

    // assert
    act.Should()
      .Throw<InvalidCurrencyNumberException>()
      .WithMessage($"The given currency {currencyNumber} number is not valid.");
  }
}
