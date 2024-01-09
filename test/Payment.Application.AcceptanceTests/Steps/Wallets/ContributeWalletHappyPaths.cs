using FluentAssertions;
using Optional;
using Payment.Application.AcceptanceTests.Contexts;
using Payment.Application.AcceptanceTests.Helpers;
using Payment.Application.Wallets.Features.ContributeWallet;
using Payment.Common.Abstraction.Models;
using Payment.Domain.ValueObjects;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace Payment.Application.AcceptanceTests.Steps.Wallets;

[Binding]
[Scope(Feature = "Contribute Wallet")]
public class ContributeWalletHappyPaths(ScenarioContext scenarioContext, TestContext testContext)
{
  private const string WalletIdKey = "WalletId";
  private const string ResponseKey = "Response";
  private const string ShareKey = "Share";
  private const string SharesKey = "Shares";

  [Given("following users in the system")]
  public async Task GivenFollowingUsersInTheSystem(Table table)
  {
    var users = table.CreateSet<UserTable>().ToList();
    users.Count.Should().BeGreaterThan(0);
    await testContext.SeedUsers(users);
  }

  [Given("following wallet with no shares in the system")]
  public async Task GivenFollowingWalletWithNoSharesInTheSystem(Table table)
  {
    var walletTable = table.CreateSet<WalletTable>().ToList();
    walletTable.Should().NotBeNull();
    walletTable.Count.Should().BeGreaterThan(0);
    await testContext.SeedWallets(walletTable);
  }

  [Given("wallet id (.*)")]
  public void GivenWalletId(Guid walletId)
  {
    scenarioContext.Add(WalletIdKey, walletId);
  }

  [Given("the following shares to be contributed to user's wallet")]
  public void GivenTheFollowingSharesToBeContributedToUsersWallet(Table table)
  {
    var shares = table.CreateSet<ShareTable>().ToList();
    shares.Should().NotBeNull();
    shares.Count.Should().BeGreaterThan(0);

    scenarioContext.Add(SharesKey, shares);
  }

  [Given("the following share to be contributed to user's wallet")]
  public void GivenTheFollowingShareToBeContributedToUsersWallet(Table table)
  {
    var share = table.CreateInstance<ShareTable>();
    share.Should().NotBeNull();

    scenarioContext.Add(ShareKey, share);
  }

  [When("I submit a single share request")]
  public async Task WhenISubmitASingleShareRequest()
  {
    var share = scenarioContext.Get<ShareTable>(ShareKey);
    var walletId = scenarioContext.Get<Guid>(WalletIdKey);

    var command = new ContributeWalletCommand
    {
      WalletId = walletId.ToString(),
      Shares = new[]
      {
        new ContributeShareCommand
        {
          Amount = share.Amount,
          Currency = share.Currency,
          ContributorId = share.ContributorId.ToString()
        }
      },
      CreatedOrModifiedBy = "TestSystem",
      CreatedOrModifiedOn = DateTime.UtcNow
    };

    var response = await testContext.WalletService.ContributeWalletAsync(command, CancellationToken.None);

    scenarioContext.Add(ResponseKey, response);
  }

  [When("I submit a multi share request")]
  public async Task WhenISubmitAMultiShareRequest()
  {
    var shares = scenarioContext.Get<List<ShareTable>>(SharesKey).ToList();
    var walletId = scenarioContext.Get<Guid>(WalletIdKey);

    var command = new ContributeWalletCommand
    {
      WalletId = walletId.ToString(),
      Shares = shares.Select(x=> new ContributeShareCommand
      {
        Amount = x.Amount,
        Currency = x.Currency,
        ContributorId = x.ContributorId.ToString()
      }),
      CreatedOrModifiedBy = "TestSystem",
      CreatedOrModifiedOn = DateTime.UtcNow
    };

    var response = await testContext.WalletService.ContributeWalletAsync(command, CancellationToken.None);

    scenarioContext.Add(ResponseKey, response);
  }

  [Then("the response should be successful")]
  public void ThenTheResponseShouldBeSuccessful()
  {
    var response = scenarioContext.Get<Option<ValidationErrorResponse, ErrorResponse>>(ResponseKey);

    response.Should().NotBeNull();
    response.HasValue.Should().BeFalse();
  }

  [Then("the wallet should have")]
  public async Task ThenTheWalletShouldHave(Table table)
  {
    var expectedWallet = table.CreateSet<WalletTable>().ToList();
    expectedWallet.Should().NotBeNull();
    expectedWallet.Count.Should().BeGreaterThan(0);

    var walletId = scenarioContext.Get<Guid>(WalletIdKey);

    var actualWallet = await testContext.WalletRepository.FindByIdAsync(WalletId.Create(walletId), CancellationToken.None);

    actualWallet.Should().NotBeNull();
    actualWallet.Shares.Should().NotBeNull();
    actualWallet.Shares.Count.Should().Be(expectedWallet.Select(x => x.Id == walletId).Count());

    var actualContributorIds = actualWallet.Shares.Select(share => share.ContributorId.Value);
    var expectedContributorIds = expectedWallet.Select(share => share.ContributorId);
    actualContributorIds.Should().Contain(expectedContributorIds);

    actualWallet.TotalSharesAmount.Should().HaveCountGreaterThan(0);
    actualWallet.TotalSharesAmount.All(x =>
        expectedWallet
          .Where(w => w.Id == walletId && w.Currency == x.Key)
          .Select(w => w.TotalSharesAmount)
          .Sum() == x.Value)
      .Should()
      .BeTrue();
  }

  [Given("the following share are already added to user's wallet")]
  public async Task GivenTheFollowingShareAreAlreadyAddedToUsersWallet(Table table)
  {
    var shares = table.CreateSet<ShareTable>().ToList();
    shares.Count.Should().BeGreaterThan(0);
    await testContext.SeedShares(shares);
  }
}
