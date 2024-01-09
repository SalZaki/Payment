using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Payment.Domain.Entities;
using Payment.Domain.ValueObjects;
using Xunit;

namespace Payment.Domain.UnitTests.TestData;

public static class WalletTestData
{
  private static readonly Fixture Fixture;
  private static readonly DateTime CreatedOn;
  private static string CreatedBy;

  static WalletTestData()
  {
    Fixture = new Fixture();
    Fixture.Customizations.Add(new RandomNumericSequenceGenerator(0, 100));
    Fixture.Customize(new AutoNSubstituteCustomization {ConfigureMembers = true});

    CreatedOn = DateTime.UtcNow;
    CreatedBy = "WalletTest-->{0}";
  }

  public static class Create
  {
    static Create()
    {
      CreatedBy = string.Format(CreatedBy, nameof(Create));
    }

    public sealed class ValidInputWithOutAmount : TheoryData<WalletId, UserId, string, DateTime>
    {
      public ValidInputWithOutAmount()
      {
        var walletId = Fixture.Create<WalletId>();
        var userId = Fixture.Create<UserId>();

        Add(walletId, userId, CreatedBy, CreatedOn);
      }
    }

    public sealed class ValidInputWithAmount : TheoryData<WalletId, UserId, Money, string, DateTime>
    {
      public ValidInputWithAmount()
      {
        var walletId = Fixture.Create<WalletId>();
        var userId = Fixture.Create<UserId>();
        var amount = Fixture.Create<decimal>();
        var currency = CurrencyTestData.DefaultCurrency;
        var money = Money.Create(amount, currency, Units.Major);

        Add(walletId, userId, money, CreatedBy, CreatedOn);
      }
    }

    public sealed class ValidInputWithMultiCurrencyShares : TheoryData<WalletId, UserId, Money, Share[], string, DateTime>
    {
      public ValidInputWithMultiCurrencyShares()
      {
        var walletId = Fixture.Create<WalletId>();
        var userId = Fixture.Create<UserId>();
        var contributorId = Fixture.Create<UserId>();
        var amount = Money.Create(Fixture.Create<decimal>(), CurrencyTestData.DefaultCurrency, Units.Major);

        Add(
          walletId,
          userId,
          amount,
          new[]
          {
            CreateShareBy(CurrencyTestData.DefaultCurrency, contributorId, walletId),
            CreateShareBy(CurrencyTestData.SecondaryCurrency, contributorId, walletId),
            CreateShareBy(CurrencyTestData.ThirdCurrency, contributorId, walletId),
            CreateShareBy(CurrencyTestData.FourthCurrency, contributorId, walletId),
            CreateShareBy(CurrencyTestData.FifthCurrency, contributorId, walletId),
          },
          CreatedBy,
          CreatedOn);
      }

      private static Share CreateShareBy(Currency currency, UserId contributorId, WalletId walletId)
      {
        var amount = Money.Create(Fixture.Create<decimal>(), currency, Units.Major);
        var shareId = Fixture.Create<ShareId>();
        return Share.Create(shareId, walletId, contributorId, amount, CreatedBy, CreatedOn);
      }
    }

    public sealed class ValidInputWithSingleCurrencyShares : TheoryData<WalletId, UserId, Money, Share[], Money, string, DateTime >
    {
      public ValidInputWithSingleCurrencyShares()
      {
        // Amount
        var amount = Money.Create(Fixture.Create<decimal>(), CurrencyTestData.DefaultCurrency, Units.Major);

        // Money
        var money1 = Money.Create(Fixture.Create<decimal>(), CurrencyTestData.DefaultCurrency, Units.Major);
        var money2 = Money.Create(Fixture.Create<decimal>(), CurrencyTestData.DefaultCurrency, Units.Major);

        // Wallet Id
        var walletId = Fixture.Create<WalletId>();

        // User Id
        var userId = Fixture.Create<UserId>();

        // Share
        var shareId1 = Fixture.Create<ShareId>();
        var share1 = Share.Create(shareId1, walletId, userId, money1, CreatedBy, CreatedOn);

        var shareId2 = Fixture.Create<ShareId>();
        var share2 = Share.Create(shareId2, walletId, userId, money2, CreatedBy, CreatedOn);

        Add(walletId, userId, amount, new[] {share1, share2}, money1 + money2, CreatedBy, CreatedOn);
      }
    }
  }

  public static class Contribute
  {
    static Contribute()
    {
      CreatedBy = string.Format(CreatedBy, nameof(Contribute));
    }

    public sealed class InvalidInputWithUserId : TheoryData<Wallet, Money, UserId, string, DateTime>
    {
      public InvalidInputWithUserId()
      {
        var userId = Fixture.Create<UserId>();
        var money = Money.Create(Fixture.Create<decimal>(), CurrencyTestData.DefaultCurrency, Units.Major);

        // Wallet
        var wallet = Wallet.Create(
          Fixture.Create<WalletId>(),
          userId,
          Money.Create(Fixture.Create<decimal>(), CurrencyTestData.DefaultCurrency, Units.Major),
          CreatedBy,
          CreatedOn);

        Add(wallet, money, userId, CreatedBy, CreatedOn);
      }
    }

    public sealed class ValidInputWithContributorId : TheoryData<Wallet, Money, UserId, string, DateTime>
    {
      public ValidInputWithContributorId()
      {
        var contributorId = Fixture.Create<UserId>();
        var money = Money.Create(Fixture.Create<decimal>(), CurrencyTestData.DefaultCurrency, Units.Major);

        var wallet = Wallet.Create(
          Fixture.Create<WalletId>(),
          Fixture.Create<UserId>(),
          Money.Create(Fixture.Create<decimal>(), CurrencyTestData.DefaultCurrency, Units.Major),
          CreatedBy,
          CreatedOn);

        Add(wallet, money, contributorId, CreatedBy, CreatedOn);
      }
    }

    public sealed class ValidInputWithContributorIdAndShareAdded : TheoryData<Wallet, Money, UserId, string, DateTime>
    {
      public ValidInputWithContributorIdAndShareAdded()
      {
        var contributorId = Fixture.Create<UserId>();
        var money = Money.Create(Fixture.Create<decimal>(), CurrencyTestData.DefaultCurrency, Units.Major);

        var wallet = Wallet.Create(
          Fixture.Create<WalletId>(),
          Fixture.Create<UserId>(),
          Money.Create(Fixture.Create<decimal>(), CurrencyTestData.DefaultCurrency, Units.Major),
          CreatedBy,
          CreatedOn);

        wallet.Contribute(money, contributorId, CreatedBy, CreatedOn);

        Add(wallet, money, contributorId, CreatedBy, CreatedOn);
      }
    }
  }
}
