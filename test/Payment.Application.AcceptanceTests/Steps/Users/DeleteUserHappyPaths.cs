using FluentAssertions;
using Optional;
using Payment.Application.AcceptanceTests.Contexts;
using Payment.Application.AcceptanceTests.Helpers;
using Payment.Application.Users.Features.DeleteUser;
using Payment.Common.Abstraction.Models;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace Payment.Application.AcceptanceTests.Steps.Users;

[Binding]
[Scope(Feature = "Delete User")]
public class DeleteUserHappyPaths(ScenarioContext scenarioContext, TestContext testContext)
{
  private const string UserIdKey = "UserId";
  private const string ResponseKey = "Response";

  [Given("following user in the system")]
  public async Task GivenFollowingUserInTheSystem(Table table)
  {
    var userTable = table.CreateSet<UserTable>().ToList();
    userTable.Should().NotBeNull();
    userTable.Count.Should().BeGreaterThan(0);
    await testContext.SeedUsers(userTable);
  }

  [Given(@"a user id ""(.*)""")]
  public void GivenAUserId(string userId)
  {
    scenarioContext.Add(UserIdKey, userId);
  }

  [When("I submit the request to delete the user")]
  public async Task WhenISubmitTheRequestToDeleteTheUser()
  {
    var userId = scenarioContext.Get<string>(UserIdKey);

    var command = new DeleteUserCommand
    {
      UserId = userId
    };

    var response = await testContext.UserService.DeleteUserAsync(command, CancellationToken.None);

    scenarioContext.Add(ResponseKey, response);
  }

  [Then("the response should be successful")]
  public void ThenTheResponseShouldBeSuccessful()
  {
    var response = scenarioContext.Get<Option<ValidationErrorResponse, ErrorResponse>>(ResponseKey);
    response.Should().NotBeNull();
    response.HasValue.Should().BeFalse();
  }
}
