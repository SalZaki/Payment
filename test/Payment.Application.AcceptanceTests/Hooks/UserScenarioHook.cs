using System.Diagnostics;
using Payment.Application.AcceptanceTests.Contexts;
using Payment.Application.Users.Features.AddFriendship;
using Payment.Application.Users.Features.CreateUser;
using Payment.Application.Users.Features.DeleteUser;
using Payment.Application.Users.Features.GetCommonFriends;
using Payment.Application.Users.Features.GetConnectionList;
using Payment.Application.Users.Features.GetUser;
using Payment.Application.Users.Services;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Infrastructure;

namespace Payment.Application.AcceptanceTests.Hooks;

[Binding]
[Scope(Tag = "user")]
public sealed class UserScenarioHook(ISpecFlowOutputHelper outputHelper)
{
  [BeforeScenario(Order = 1)]
  public void BeforeScenario(ScenarioContext scenarioContext, TestContext testContext)
  {
    scenarioContext.Clear();
    scenarioContext.Add(scenarioContext.ScenarioInfo.Title, Stopwatch.StartNew());
    outputHelper.WriteLine("----------------------------------------------------------------------------------");
    outputHelper.WriteLine($"Before Scenario: Title - {scenarioContext.ScenarioInfo.Title}");
    outputHelper.WriteLine($"Before Scenario: Status - {scenarioContext.ScenarioExecutionStatus.ToString()}");
    outputHelper.WriteLine($"Before Scenario: Started - {DateTimeOffset.Now:HH:mm:ss.fff}");

    testContext.UserService = new UserService(
      GetUserQueryHandlerFactory(testContext),
      CreateUserCommandHandlerFactory(testContext),
      AddFriendCommandHandlerFactory(testContext),
      GetCommonFriendsQueryHandlerFactory(testContext),
      DeleteUserCommandHandlerFactory(testContext),
      GetConnectionListQueryHandlerFactory(testContext));
  }

  [AfterScenario(Order = 1)]
  public async Task AfterScenario(ScenarioContext scenarioContext, TestContext testContext)
  {
    await testContext.UserRepository.CleanUpUsersAsync();
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

  private static GetUserQueryHandler GetUserQueryHandlerFactory(TestContext testContext)
  {
    return new GetUserQueryHandler(testContext.UserRepository, new GetUserQueryValidator(),
      new UserToGetUserResponseMapper());
  }

  private static CreateUserCommandHandler CreateUserCommandHandlerFactory(TestContext testContext)
  {
    return new CreateUserCommandHandler(testContext.UserRepository,
      new CreateUserCommandValidator(testContext.UserRepository));
  }

  private static AddFriendshipCommandHandler AddFriendCommandHandlerFactory(TestContext testContext)
  {
    return new AddFriendshipCommandHandler(testContext.UserRepository,
      new AddFriendshipCommandValidator(testContext.UserRepository));
  }

  private static GetCommonFriendsQueryHandler GetCommonFriendsQueryHandlerFactory(TestContext testContext)
  {
    return new GetCommonFriendsQueryHandler(testContext.UserRepository,
      new GetCommonFriendsQueryValidator(testContext.UserRepository));
  }

  private static DeleteUserCommandHandler DeleteUserCommandHandlerFactory(TestContext testContext)
  {
    return new DeleteUserCommandHandler(testContext.UserRepository,
      new DeleteUserCommandValidator());
  }

  private static GetConnectionListQueryHandler GetConnectionListQueryHandlerFactory(TestContext testContext)
  {
    return new GetConnectionListQueryHandler(testContext.UserRepository,
      new GetConnectionListQueryValidator(testContext.UserRepository));
  }
}
