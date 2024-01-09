using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Infrastructure;
using System.Diagnostics;
using Payment.Application.AcceptanceTests.Contexts;
using Payment.Application.Wallets.Features.ContributeWallet;
using Payment.Application.Wallets.Features.CreateWallet;
using Payment.Application.Wallets.Features.GetWallet;
using Payment.Application.Wallets.Services;

namespace Payment.Application.AcceptanceTests.Hooks;

[Binding]
[Scope(Tag = "wallet")]
public sealed class WalletScenarioHook(ISpecFlowOutputHelper outputHelper)
{
  [BeforeScenario(Order = 1)]
  public void BeforeScenario(ScenarioContext scenarioContext, TestContext testContext)
  {
    scenarioContext.Clear();
    scenarioContext.Add(scenarioContext.ScenarioInfo.Title, Stopwatch.StartNew());
    outputHelper.WriteLine("----------------------------------------------------------------------------------");
    outputHelper.WriteLine($"Before Scenario: Title [{scenarioContext.ScenarioInfo.Title}]");
    outputHelper.WriteLine($"Before Scenario: Status [{scenarioContext.ScenarioExecutionStatus.ToString()}]");
    testContext.WalletService = new WalletService(
      CreateWalletCommandHandlerFactory(testContext),
      GetWalletQueryHandlerFactory(testContext),
      ContributeWalletCommandHandlerFactory(testContext));
  }

  [AfterScenario(Order = 1)]
  public async Task AfterScenario(ScenarioContext scenarioContext, TestContext testContext)
  {
    await testContext.WalletRepository.CleanUpWalletsAsync();
    var elapsed = scenarioContext.Get<Stopwatch>(scenarioContext.ScenarioInfo.Title).Elapsed;
    scenarioContext.Remove(scenarioContext.ScenarioInfo.Title);
    outputHelper.WriteLine($"After Scenario: Title [{scenarioContext.ScenarioInfo.Title}]");
    outputHelper.WriteLine($"After Scenario: Status [{scenarioContext.ScenarioExecutionStatus.ToString()}]");
    outputHelper.WriteLine($"After Scenario: Time Elapsed [{elapsed.Hours:D2}:{elapsed.Minutes:D2}:{elapsed.Seconds:D2}.{elapsed.Milliseconds}]");
    outputHelper.WriteLine("----------------------------------------------------------------------------------");
  }

  [BeforeStep]
  public void BeforeStep(ScenarioContext scenarioContext)
  {
    scenarioContext.Add(scenarioContext.StepContext.StepInfo.Text, Stopwatch.StartNew());
    outputHelper.WriteLine("----------------------------------------------------------------------------------");
    outputHelper.WriteLine($"Before Step: Title [{scenarioContext.StepContext.StepInfo.Text}]");
    outputHelper.WriteLine($"Before Step: Status [{scenarioContext.StepContext.Status.ToString()}]");
    outputHelper.WriteLine($"Before Step: Started [{DateTimeOffset.Now:HH:mm:ss.fff}]");
  }

  [AfterStep]
  public void AfterStep(ScenarioContext scenarioContext)
  {
    var elapsed = scenarioContext.Get<Stopwatch>(scenarioContext.StepContext.StepInfo.Text).Elapsed;
    scenarioContext.Remove(scenarioContext.StepContext.StepInfo.Text);
    outputHelper.WriteLine($"After Step: Title [{scenarioContext.StepContext.StepInfo.Text}]");
    outputHelper.WriteLine($"After Step: Status [{scenarioContext.StepContext.Status.ToString()}]");
    outputHelper.WriteLine($"After Step: Time Elapsed [{elapsed.Hours:D2}:{elapsed.Minutes:D2}:{elapsed.Seconds:D2}.{elapsed.Milliseconds}]");
    outputHelper.WriteLine("----------------------------------------------------------------------------------");
  }

  private static CreateWalletCommandHandler CreateWalletCommandHandlerFactory(TestContext testContext)
  {
    return new CreateWalletCommandHandler(testContext.WalletRepository,
      new CreateWalletCommandValidator(testContext.UserRepository));
  }

  private static GetWalletQueryHandler GetWalletQueryHandlerFactory(TestContext testContext)
  {
    return new GetWalletQueryHandler(testContext.WalletRepository,
      new GetWalletQueryValidator(),
      new WalletToGetWalletResponseMapper());
  }

  private static ContributeWalletCommandHandler ContributeWalletCommandHandlerFactory(TestContext testContext)
  {
    return new ContributeWalletCommandHandler(testContext.WalletRepository,
      new ContributeWalletCommandValidator(testContext.WalletRepository));
  }
}
