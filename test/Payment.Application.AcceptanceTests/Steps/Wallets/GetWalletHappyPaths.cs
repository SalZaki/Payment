using FluentAssertions;
using Payment.Application.AcceptanceTests.Contexts;
using Payment.Application.AcceptanceTests.Helpers;
using Payment.Application.Wallets.Features.GetWallet;
using Payment.Common.Abstraction.Models;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using OneOf;

namespace Payment.Application.AcceptanceTests.Steps.Wallets;

[Binding]
[Scope(Feature = "Get Wallet")]
public class GetWalletHappyPaths(ScenarioContext scenarioContext, TestContext testContext)
{
  private const string WalletRequestKey = "WalletRequest";
  private const string ResponseKey = "Response";
  private const string GetWalletResponseResponseKey = "GetWalletResponseResponse";

  [Given("following wallets in the system")]
  public async Task GivenFollowingWalletsInTheSystem(Table table)
  {
    var walletTable = table.CreateSet<WalletTable>().ToList();

    walletTable.Should().NotBeNull();
    walletTable.Count.Should().BeGreaterThan(0);
    await testContext.SeedWallets(walletTable);
  }

  [Given("following shares for a wallet in the system")]
  public async Task GivenFollowingSharesForAWalletInTheSystem(Table table)
  {
    var shares = table.CreateSet<ShareTable>().ToList();

    shares.Should().NotBeNull();
    shares.Count.Should().BeGreaterThan(0);
    await testContext.SeedShares(shares);
  }

  [Given(@"a wallet id ""(.*)"" with shares")]
  [Given(@"a wallet id ""(.*)"" with no shares")]
  public void GivenAWalletIdWithOrWithoutShares(string walletId)
  {
    var query = new GetWalletQuery
    {
      WalletId = walletId
    };

    scenarioContext.Add(WalletRequestKey, query);
  }

  [When("I submit the request to get a wallet")]
  public async Task WhenISubmitTheRequestToGetAWallet()
  {
    var request = scenarioContext.Get<GetWalletQuery>(WalletRequestKey);

    var response = await testContext.WalletService.GetWalletAsync(request, CancellationToken.None);

    scenarioContext.Add(ResponseKey, response);
  }

  [Then("the response should be successful")]
  public void ThenTheResponseShouldBeSuccessful()
  {
    var response = scenarioContext.Get<OneOf<GetWalletResponse, ValidationErrorResponse, ErrorResponse>>(ResponseKey);

    response.Should().NotBeNull();
    response.Value.Should().BeOfType<GetWalletResponse>();
    response.IsT0.Should().BeTrue();

    scenarioContext.Add(GetWalletResponseResponseKey, response.AsT0);
  }

  [Then("the wallet should have")]
  public void ThenTheWalletShouldHave(Table table)
  {
    var expectedWallet = table.CreateInstance<WalletTable>();

    expectedWallet.Should().NotBeNull();

    var actualWallet = scenarioContext.Get<GetWalletResponse>(GetWalletResponseResponseKey);
    actualWallet.Should().NotBeNull();
    actualWallet.WalletId.Should().Be(expectedWallet.Id.ToString());
    actualWallet.Amount.Should().Be(expectedWallet.Amount);
    actualWallet.UserId.Should().Be(expectedWallet.UserId.ToString());

    actualWallet.Shares.Should().NotBeNull();

    if (actualWallet.Shares?.Count > 0)
    {
      actualWallet.Shares?.Count.Should().Be(expectedWallet.ShareCount);
      actualWallet.Shares?.Any(x => x.ContributorId == expectedWallet.ContributorId.ToString())
        .Should()
        .BeTrue();

      actualWallet.TotalSharesAmount.Should().HaveCountGreaterThan(0);
      actualWallet.TotalSharesAmount?.Any(x => x.Value == expectedWallet.TotalSharesAmount)
        .Should()
        .BeTrue();
    }
  }
}
