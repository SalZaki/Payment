using AutoFixture;
using AutoFixture.Idioms;
using FluentAssertions;
using Payment.Common.Abstraction.Domain.Exceptions;
using Payment.Domain.Entities;
using Payment.Domain.UnitTests.TestData;
using Payment.Domain.UnitTests.ValueObjects;
using Payment.Domain.ValueObjects;
using Xunit;

namespace Payment.Domain.UnitTests.Entities;

/// <summary>
/// Could filter by `dotnet test --filter "Category=Wallet"` in running tests in command line
/// </summary>
[Trait("Category", "Wallet")]
public sealed class WalletTests : BaseTest
{
  [Theory(DisplayName = "Constructor --> with guard clauses assertion")]
  [Trait("Category", "Wallet.Constructor")]
  [UserAutoNSubstituteData]
  public void Constructor_ShouldHaveGuardClauses(GuardClauseAssertion assertion)
  {
    // act & assert
    assertion.Verify(typeof(Wallet).GetConstructors());
  }

  [Fact(DisplayName = "Create --> with null wallet id then should fail")]
  [Trait("Category", "Wallet.Create")]
  public void Create_WithNullWalletId_ShouldThrowException()
  {
    // arrange
    var walletId = null as WalletId;
    var ownerId = Fixture.Create<UserId>();

    // act
    var act = () => { Wallet.Create(walletId!, ownerId); };

    // assert
    act.Should()
      .Throw<ArgumentNullException>().Where(x =>
        x.Message.StartsWith("WalletId can not be null."))
      .WithParameterName(nameof(walletId));
  }

  [Fact(DisplayName = "Create --> with null user id then should fail")]
  [Trait("Category", "Wallet.Create")]
  public void Create_WithNullOwnerId_ShouldThrowException()
  {
    // arrange
    var walletId = Fixture.Create<WalletId>();
    var userId = null as UserId;

    // act
    var act = () => { Wallet.Create(walletId, userId!); };

    // assert
    act.Should()
      .Throw<ArgumentNullException>().Where(x =>
        x.Message.StartsWith("UserId can not be null."))
      .WithParameterName(nameof(userId));
  }

  [Theory(DisplayName = "Create --> with valid inputs with out amount then should create")]
  [Trait("Category", "Wallet.Create")]
  [ClassData(typeof(WalletTestData.Create.ValidInputWithOutAmount))]
  public void Create_WithOutAmount_ShouldSucceed(
    WalletId walletId,
    UserId userId,
    string createdBy,
    DateTime createdOn)
  {
    // act
    var sut = Wallet.Create(walletId, userId, null, createdBy, createdOn);

    // assert
    sut.Should().NotBeNull();
    sut.Id.Should().Be(walletId);
    sut.UserId.Should().Be(userId);
    sut.Shares.Should().NotBeNull();
    sut.Shares.Count.Should().Be(0);
    sut.CreatedBy.Should().Be(createdBy);
    sut.CreatedOn.Should().BeCloseTo(DateTime.UtcNow, precision: TimeSpan.FromSeconds(3));
    sut.TotalSharesAmount.Should().NotBeNull();
    sut.TotalSharesAmount.Count.Should().Be(0);
  }

  [Theory(DisplayName = "Create --> with valid inputs with amount then should create")]
  [Trait("Category", "Wallet.Create")]
  [ClassData(typeof(WalletTestData.Create.ValidInputWithAmount))]
  public void Create_WithAmount_ShouldSucceed(
    WalletId walletId,
    UserId userId,
    Money amount,
    string createdBy,
    DateTime createdOn)
  {
    // act
    var sut = Wallet.Create(walletId, userId, amount, createdBy, createdOn);

    // assert
    sut.Should().NotBeNull();
    sut.Id.Should().Be(walletId);
    sut.UserId.Should().Be(userId);
    sut.Amount.Should().Be(amount);
    sut.Shares.Should().NotBeNull();
    sut.Shares.Count.Should().Be(0);
    sut.CreatedBy.Should().Be(createdBy);
    sut.CreatedOn.Should().BeCloseTo(DateTime.UtcNow, precision: TimeSpan.FromSeconds(3));
    sut.TotalSharesAmount.Should().NotBeNull();
    sut.TotalSharesAmount.Count.Should().Be(0);
  }

  [Theory(DisplayName = "Contribute --> with invalid inputs then should fail")]
  [Trait("Category", "Wallet.Contribute")]
  [ClassData(typeof(WalletTestData.Contribute.InvalidInputWithUserId))]
  public void Contribute_WithUserId_ShouldThrowException(
    Wallet sut,
    Money amount,
    UserId userId,
    string createdBy,
    DateTime createdOn)
  {
    // act
    var act = () => { sut.Contribute(amount, userId, createdBy, createdOn); };

    // assert
    act.Should()
      .Throw<BusinessPolicyValidationException>()
      .WithMessage($"User with id {userId} cannot contribute to its own wallet");
  }

  [Theory(DisplayName = "Contribute --> with valid inputs then should add")]
  [Trait("Category", "Wallet.Contribute")]
  [ClassData(typeof(WalletTestData.Contribute.ValidInputWithContributorId))]
  public void Contribute_WithContributorId_ShouldAdd(
    Wallet sut,
    Money amount,
    UserId contributorId,
    string createdBy,
    DateTime createdOn)
  {
    // act
    sut.Contribute(amount, contributorId, createdBy, createdOn);

    // assert
    sut.Shares.Should().NotBeNull();
    sut.Shares.Count.Should().Be(1);
    sut.CreatedBy.Should().Be(createdBy);
    sut.CreatedOn.Should().BeCloseTo(DateTime.UtcNow, precision: TimeSpan.FromSeconds(5));
    sut.TotalSharesAmount.Should().NotBeNull();
    sut.TotalSharesAmount.Count.Should().Be(1);
    sut.TotalSharesAmount.All(x =>
        sut.Shares
          .Where(s => s.Amount.Currency.Code == x.Key)
          .Select(w => w.Amount.InMajorUnits)
          .Sum() == x.Value)
      .Should()
      .BeTrue();
  }

  [Theory(DisplayName = "Contribute --> with valid inputs then should contribute")]
  [Trait("Category", "Wallet.Contribute")]
  [ClassData(typeof(WalletTestData.Contribute.ValidInputWithContributorIdAndShareAdded))]
  public void Contribute_WithContributorId_ShouldContribute(
    Wallet sut,
    Money amount,
    UserId contributorId,
    string createdBy,
    DateTime createdOn)
  {
    // act
    sut.Contribute(amount, contributorId, createdBy, createdOn);

    // assert
    sut.Shares.Should().NotBeNull();
    sut.Shares.Count.Should().Be(1);
    sut.CreatedBy.Should().Be(createdBy);
    sut.CreatedOn.Should().BeCloseTo(DateTime.UtcNow, precision: TimeSpan.FromSeconds(5));
    sut.TotalSharesAmount.Should().NotBeNull();
    sut.TotalSharesAmount.Count.Should().Be(1);
    sut.TotalSharesAmount.All(x =>
        sut.Shares
          .Where(s => s.Amount.Currency.Code == x.Key)
          .Select(w => w.Amount.InMajorUnits)
          .Sum() == x.Value)
      .Should()
      .BeTrue();
  }
}
