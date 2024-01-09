using AutoFixture;
using AutoFixture.AutoNSubstitute;

namespace Payment.Domain.UnitTests.Entities;

public abstract class BaseTest
{
  protected Fixture Fixture { get; }

  protected BaseTest()
  {
    Fixture = new Fixture();
    Fixture.Customizations.Add(new RandomNumericSequenceGenerator(0, 100000));
    Fixture.Customize(new AutoNSubstituteCustomization {ConfigureMembers = true});
  }
}
