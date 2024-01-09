using AutoFixture.Idioms;
using FluentAssertions;
using Payment.Domain.Exceptions;
using Payment.Domain.ValueObjects;
using Xunit;

namespace Payment.Domain.UnitTests.ValueObjects;

[Trait("Class", "FullName")]
public sealed class FullNameTests
{
  [Theory]
  [MoneyAutoNSubstituteData]
  [Trait("Method", "Constructor")]
  public void should_have_constructor_guard_clauses(GuardClauseAssertion assertion)
  {
    // act & assert
    assertion.Verify(typeof(FullName).GetConstructors());
  }

  [Theory]
  [Trait("Method", "Create")]
  [InlineData("")]
  [InlineData(" ")]
  public void should_throw_exception_when_creating_given_null_or_empty_value(string value)
  {
    // act
    var act = () => { FullName.Create(value); };

    // assert
    act.Should()
      .Throw<InvalidFullNameException>()
      .Where(x => x.Message.StartsWith($"FullName '{value}' is invalid."));
  }

  [Theory]
  [Trait("Method", "FullName.Create")]
  [InlineData("Martin Lowe", "Martin Lowe")]
  [InlineData(" Sebastian Cole", "Sebastian Cole")]
  [InlineData(" Mohammed Ali", "Mohammed Ali")]
  [InlineData("  Billie Jean       ", "Billie Jean")]
  public void should_create_given_invalid_value(string actualValue, string expectedValue)
  {
    // act
    var actualFullname = FullName.Create(actualValue);

    // assert
    actualFullname.Should().NotBeNull();
    actualFullname.Value.Should().Be(expectedValue);
    actualFullname.Value.Length.Should().Be(expectedValue.Length);
  }
}
