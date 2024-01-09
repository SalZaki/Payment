using FluentAssertions;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using OneOf;
using Payment.Common.Abstraction.Models;
using Payment.Application.AcceptanceTests.Contexts;
using Payment.Application.AcceptanceTests.Helpers;
using Payment.Application.Wallets.Features.CreateWallet;

namespace Payment.Application.AcceptanceTests.Steps.Wallets;

[Binding]
[Scope(Feature = "Create Wallet")]
public class CreateWalletHappyPaths(ScenarioContext scenarioContext, TestContext testContext)
{
  private const string UserIdKey = "UserId";
  private const string AmountKey = "Amount";
  private const string CurrencyKey = "Currency";
  private const string ResponseKey = "Response";
  private const string WalletIdKey = "WalletId";

  [Given("the following users exists in the system")]
  public async Task GivenTheFollowingUsersExistsInTheSystem(Table table)
  {
    var users = table.CreateSet<UserTable>().ToList();
    users.Count.Should().BeGreaterThan(0);
    await testContext.SeedUsers(users);
  }

  [Given("there are no wallets")]
  public async Task GivenThereAreNoWalletsExistForThem()
  {
    await testContext.WalletRepository.CleanUpWalletsAsync();
    var count = await testContext.WalletRepository.TotalWalletsAsync();
    count.Should().Be(0);
  }

  [Given("a user id (.*)")]
  public void GivenAUserId(Guid userId)
  {
    scenarioContext.Add(UserIdKey, userId);
  }

  [Given("currency (.*) and amount (.*)")]
  public void GivenCurrencyAndAmount(string currency, decimal amount)
  {
    scenarioContext.Add(CurrencyKey, currency);
    scenarioContext.Add(AmountKey, amount);
  }

  [When("I submit the request")]
  public async Task WhenISubmitTheRequest()
  {
    var userId = scenarioContext.Get<Guid>(UserIdKey);
    scenarioContext.TryGetValue<string>(CurrencyKey, out var currency);
    scenarioContext.TryGetValue<decimal>(AmountKey, out var amount);
    var createWalletRequest = new CreateWalletCommand
    {
      UserId = userId.ToString(),
      Currency = currency,
      Amount = amount
    };

    var response = await testContext.WalletService.CreateWalletAsync(createWalletRequest, CancellationToken.None);

    scenarioContext.Add(ResponseKey, response);
  }

  [Then("the response should be successful")]
  public void ThenTheResponseShouldBeSuccessful()
  {
    var response = scenarioContext.Get<OneOf<CreateWalletResponse, ValidationErrorResponse, ErrorResponse>>(ResponseKey);

    response.Should().NotBeNull();
    response.Value.Should().BeOfType<CreateWalletResponse>();
    response.IsT0.Should().BeTrue();
  }

  [Then("the response should have a newly created wallet id")]
  public void ThenTheResponseShouldHaveANewlyCreatedWalletId()
  {
    var response = scenarioContext.Get<OneOf<CreateWalletResponse, ValidationErrorResponse, ErrorResponse>>(ResponseKey);

    response.Should().NotBeNull();
    response.AsT0.WalletId.Should().NotBeNullOrEmpty();

    scenarioContext.Add(WalletIdKey, Guid.Parse(response.AsT0.WalletId));
  }

  [Then("the wallet should have")]
  public async Task ThenTheWalletShouldHave(Table table)
  {
    var expectedWallet = table.CreateInstance<WalletTable>();
    expectedWallet.Should().NotBeNull();

    var walletId = scenarioContext.Get<Guid>(WalletIdKey);
    var actualWallet = await testContext.WalletRepository.FindOneAsync(x => x.Id == walletId, CancellationToken.None);

    actualWallet.Should().NotBeNull();
    actualWallet.Amount.Should().NotBeNull();
    actualWallet.Amount.InMajorUnits.Should().Be(expectedWallet.Amount);
    actualWallet.Shares.Should().NotBeNull();
    actualWallet.Shares.Count.Should().Be(expectedWallet.ShareCount);
  }
}
