using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Payment.Domain.Entities;
using Payment.Domain.Exceptions;
using Payment.Domain.ValueObjects;
using Xunit;

namespace Payment.Domain.UnitTests.ValueObjects;

/// <summary>
/// The British pound sterling has 2 decimals. £10.00 is thus expressed as 1000
/// The Euro has 2 decimals. €10.00 is thus expressed as 1000
/// The US dollar has 2 decimals. $10.00 is thus expressed as 1000
/// The Tunisian dinar has 3 decimals. TND 10.000 is thus expressed as 10000
/// The Japanese yen has no decimals. ¥10 is thus expressed as 10
/// </summary>
/// <remarks> https://docs.numeral.io/v2022-01-01/docs/amounts-currencies-guide </remarks>
[Trait("Class", "Money")]
public sealed class MoneyTests
{
  [Theory]
  [MoneyAutoNSubstituteData]
  [Trait("Method", "Money.Constructor")]
  public void should_have_constructor_guard_clauses(GuardClauseAssertion assertion)
  {
    // act & assert
    assertion.Verify(typeof(Money).GetConstructors());
  }

  [Theory]
  [Trait("Method", "Money.Create")]
  [InlineData("GBP", 100.00, 100.00, 10000)]
  [InlineData("EUR", 23.00, 23.00, 2300)]
  [InlineData("USD", 56.00, 56.00, 5600)]
  [InlineData("TND", 15.000, 15.000, 15000)]
  public void should_resolve_major_units_of_a_currency(
    string currencyCode,
    decimal value,
    decimal expectedMajor,
    decimal expectedMinor)
  {
    // arrange
    var units = Units.Major;
    var currency = Currency.Parse(currencyCode);

    // act
    var money = Money.Create(value, currency, units);

    // asserts
    money.InMajorUnits.Should().Be(expectedMajor);
    money.InMinorUnits.Should().Be(expectedMinor);
  }

  [Theory]
  [Trait("Method", "Money.Create")]
  [InlineData("GBP", 100, 1, 100)]
  [InlineData("GBP", 120.23, 1.2023, 120.23)]
  [InlineData("USD", 80, 0.80, 80)]
  [InlineData("JPY", 100, 100, 100)]
  public void should_resolve_minor_units_of_currency(
    string currencyCode,
    decimal amount,
    decimal expectedMajor,
    decimal expectedMinor)
  {
    // arrange
    var units = Units.Minor;
    var currency = Currency.Parse(currencyCode);

    // act
    var money = Money.Create(amount, currency, units);

    // assert
    money.InMajorUnits.Should().Be(expectedMajor);
    money.InMinorUnits.Should().Be(expectedMinor);
  }

  [Theory]
  [Trait("Method", "Money.Create")]
  [InlineData("GBP", 0,0,0)]
  [InlineData("USD", 0,0,0)]
  public void GivenZeroAmount_WhenCreatingMoney_ThenShouldCreate(
    string currencyCode,
    decimal amount,
    decimal expectedMajor,
    decimal expectedMinor)
  {
    // arrange
    var units = Units.Major;
    var currency = Currency.Parse(currencyCode);

    // act
    var actualResponse = Money.Create(amount, currency, units);

    // assert
    actualResponse.Should().NotBeNull();
    actualResponse.InMajorUnits.Should().Be(expectedMajor);
    actualResponse.InMinorUnits.Should().Be(expectedMinor);
  }

  [Theory]
  [Trait("Method", "Money.Create")]
  [InlineData("USD", -1)]
  [InlineData("ETB", -1.29)]
  [InlineData("EUR", -34)]
  [InlineData("TRY", -134.00)]
  public void GivenNegativeOrZeroAmount_WhenCreate_ThenShouldThrowException(
    string currencyCode,
    decimal amount)
  {
    // arrange
    var units = Units.Major;
    var currency = Currency.Parse(currencyCode);

    // act
    var act = () => { Money.Create(amount, currency, units); };

    // assert
    act.Should()
      .Throw<NegativeAmountException>()
      .WithMessage($"The given amount {amount} is negative.");
  }

  [Theory]
  [Trait("Method", "Money.Create")]
  [InlineData("GBP", 1203940)]
  [InlineData("USD", 499828390.30)]
  [InlineData("TRY", 234906775.44)]
  [InlineData("EUR", 2000000.00)]
  public void GivenMaxAmountInput_WhenCreate_ThenShouldThrowException(
    string currencyCode,
    decimal amount)
  {
    // arrange
    var units = Units.Minor;
    var currency = Currency.Parse(currencyCode);

    // act
    var act = () => { Money.Create(amount, currency, units); };

    // assert
    act.Should()
      .Throw<MaxAmountException>()
      .WithMessage($"The given amount {amount} is more than maximum amount allowed.");
  }

  [Theory]
  [Trait("Method", "Money.SetAmount")]
  [InlineData("GBP", 123, 34, 0.34, 34)]
  public void GivenValidInputs_WhenSetAmount_ThenShouldSet(
    string currencyCode,
    decimal oldAmount,
    decimal newAmount,
    decimal expectedMajor,
    decimal expectedMinor)
  {
    // arrange
    var units = Units.Minor;
    var currency = Currency.Parse(currencyCode);
    var money = Money.Create(oldAmount, currency, units);

    // act
    money.SetAmount(newAmount);

    // assert
    money.InMajorUnits.Should().Be(expectedMajor);
    money.InMinorUnits.Should().Be(expectedMinor);
  }

  [Theory]
  [Trait("Method", "Money.UpdateAmount")]
  [InlineData("GBP", 2300, 10, 23.10, 2310)]
  public void GivenValidInputs_WhenUpdateAmount_ThenShouldUpdate(
    string currencyCode,
    decimal amount,
    decimal addAmount,
    decimal expectedMajor,
    decimal expectedMinor)
  {
    // arrange
    var units = Units.Minor;
    var currency = Currency.Parse(currencyCode);
    var money = Money.Create(amount, currency, units);

    // act
    money.UpdateAmount(addAmount);

    // assert
    money.InMajorUnits.Should().Be(expectedMajor);
    money.InMinorUnits.Should().Be(expectedMinor);
  }

  [Theory]
  [Trait("Method", "Money.AddOperator")]
  [InlineData("GBP", 2300, 10, 23.10, 2310)]
  public void GivenValidInputs_WhenAddOperator_ThenShouldAdd(
    string currencyCode,
    decimal amount,
    decimal addAmount,
    decimal expectedMajor,
    decimal expectedMinor)
  {
    // arrange
    var units = Units.Minor;
    var currency = Currency.Parse(currencyCode);
    var a = Money.Create(amount, currency, units);
    var b = Money.Create(addAmount, currency, units);

    // act
    var actualResult = a + b;

    // assert
    actualResult.InMajorUnits.Should().Be(expectedMajor);
    actualResult.InMinorUnits.Should().Be(expectedMinor);
  }

  [Theory]
  [Trait("Method", "Money.SubtractOperator")]
  [InlineData("GBP", 2300, 10, 22.90, 2290)]
  public void GivenValidInputs_WhenSubtractOperator_ThenShouldSubtract(
    string currencyCode,
    decimal amount,
    decimal subtractAmount,
    decimal expectedMajor,
    decimal expectedMinor)
  {
    // arrange
    var units = Units.Minor;
    var currency = Currency.Parse(currencyCode);
    var a = Money.Create(amount, currency, units);
    var b = Money.Create(subtractAmount, currency, units);

    // act
    var actualResult = a - b;

    // assert
    actualResult.Should().NotBeNull();
    actualResult.InMajorUnits.Should().Be(expectedMajor);
    actualResult.InMinorUnits.Should().Be(expectedMinor);
  }
}

public sealed class MoneyAutoNSubstituteDataAttribute(string currencyCode = "GBP") : AutoDataAttribute(() =>
{
  var fixture = new Fixture();
  fixture.Customize(new CurrencyCustomization(currencyCode));
  fixture.Customize(new AutoNSubstituteCustomization {ConfigureMembers = true});
  return fixture;
});

public sealed class UserAutoNSubstituteDataAttribute(
  string id = "5D027FB1-7B3C-4495-A59B-8D283C2261EC",
  string fullname = "Super Tester") : AutoDataAttribute(() =>
{
  var fixture = new Fixture();
  fixture.Customize(new UserCustomization(Guid.Parse(id), fullname));
  fixture.Customize(new AutoNSubstituteCustomization {ConfigureMembers = true});
  return fixture;
});

public sealed class UserCustomization(Guid userId, string fullName) : ICustomization
{
  public void Customize(IFixture fixture)
  {
    ArgumentNullException.ThrowIfNull(fixture);

    fixture.Register(() => User.Create(UserId.Create(userId), FullName.Create(fullName)));
  }
}

public sealed class CurrencyCustomization(string currencyCode) : ICustomization
{
  public void Customize(IFixture fixture)
  {
    ArgumentNullException.ThrowIfNull(fixture);

    fixture.Register(() => Currency.Parse(currencyCode));
  }
}
