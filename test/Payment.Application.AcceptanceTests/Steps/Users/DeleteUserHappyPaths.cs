using FluentAssertions;
using Optional;
using Payment.Application.AcceptanceTests.Contexts;
using Payment.Application.AcceptanceTests.Helpers;
using Payment.Application.Users.Features.DeleteUser;
using Payment.Common.Abstraction.Models;
using Payment.Domain.ValueObjects;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace Payment.Application.AcceptanceTests.Steps.Users;

[Binding]
[Scope(Feature = "Delete User")]
public class DeleteUserHappyPaths(ScenarioContext scenarioContext, TestContext testContext)
{
  private const string UserIdKey = "UserId";
  private const string FriendIdKey = "FriendId";
  private const string ResponseKey = "Response";

  [Given("following users in the system")]
  public async Task GivenFollowingUsersInTheSystem(Table table)
  {
    var userTable = table.CreateSet<UserTable>().ToList();
    userTable.Should().NotBeNull();
    userTable.Count.Should().BeGreaterThan(0);
    await testContext.SeedUsers(userTable);
  }

  [Given("the user has the following friend in the system")]
  public async Task GivenTheUserHasTheFollowingFriendInTheSystem(Table table)
  {
    var friendshipTable = table.CreateSet<FriendshipTable>().ToList();
    friendshipTable.Should().NotBeNull();
    friendshipTable.Count.Should().BeGreaterThan(0);
    await testContext.SeedFriends(friendshipTable);
  }

  [Given("a user id (.*)")]
  public void GivenAUserId(Guid userId)
  {
    scenarioContext.Add(UserIdKey, userId);
  }

  [When("I submit the request to delete the user")]
  public async Task WhenISubmitTheRequestToDeleteTheUser()
  {
    var userId = scenarioContext.Get<Guid>(UserIdKey);

    var command = new DeleteUserCommand
    {
      UserId = userId.ToString()
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

  [Then("the user's friendship with user id (.*) should also be deleted")]
  public async Task ThenTheUsersFriendshipWithUserIdShouldAlsoBeDeleted(Guid id)
  {
    var friendId = UserId.Create(id);
    var userId = UserId.Create(scenarioContext.Get<Guid>(UserIdKey));

    var friend = await testContext.UserRepository.FindByIdAsync(friendId, CancellationToken.None);

    friend.Friendships.Should().NotBeNull();
    friend.Friendships.Any(x => x.Friend.Id == userId).Should().BeFalse();
  }
}
