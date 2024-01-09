using FluentAssertions;
using OneOf;
using Payment.Application.AcceptanceTests.Contexts;
using Payment.Application.AcceptanceTests.Helpers;
using Payment.Application.Users.Features.GetCommonFriends;
using Payment.Common.Abstraction.Models;
using System.Collections.ObjectModel;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace Payment.Application.AcceptanceTests.Steps.Users;

[Binding]
[Scope(Feature = "Get Common Friends")]
public class GetCommonFriendsHappyPaths(ScenarioContext scenarioContext, TestContext testContext)
{
  private const string UserId1Key = "UserId1";
  private const string UserId2Key = "UserId2";
  private const string ResponseKey = "Response";
  private const string GetCommonFriendsResponseKey = "GetCommonFriendsResponse";

  [Given("following users in the system")]
  public async Task GivenFollowingUsersInTheSystem(Table table)
  {
    var userTable = table.CreateSet<UserTable>().ToList();
    userTable.Should().NotBeNull();
    userTable.Count.Should().BeGreaterThan(0);
    await testContext.SeedUsers(userTable);
  }

  [Given("the users has the following friends in the system")]
  public async Task GivenTheUsersHasTheFollowingFriendsInTheSystem(Table table)
  {
    var friendshipTable = table.CreateSet<FriendshipTable>().ToList();
    friendshipTable.Should().NotBeNull();
    friendshipTable.Count.Should().BeGreaterThan(0);
    await testContext.SeedFriends(friendshipTable);
  }

  [Given(@"a user1 with id ""(.*)""")]
  public void GivenAUser1WithId(string userId)
  {
    scenarioContext.Add(UserId1Key, userId);
  }

  [Given(@"a user2 with id ""(.*)""")]
  public void GivenAUser2WithId(string userId)
  {
    scenarioContext.Add(UserId2Key, userId);
  }

  [When("I submit the request to get common friends")]
  public async Task WhenISubmitTheRequestToGetCommonFriends()
  {
    var userId1 = scenarioContext.Get<string>(UserId1Key);
    var userId2 = scenarioContext.Get<string>(UserId2Key);

    var query = new GetCommonFriendsQuery
    {
      UserId1 = userId1,
      UserId2 = userId2
    };

    var response = await testContext.UserService.GetCommonFriendsAsync(query, CancellationToken.None);

    scenarioContext.Add(ResponseKey, response);
  }

  [Then("the response should be successful")]
  public void ThenTheResponseShouldBeSuccessful()
  {
    var response = scenarioContext.Get<OneOf<IReadOnlyCollection<GetCommonFriendsResponse>, ValidationErrorResponse, ErrorResponse>>(ResponseKey);
    response.Should().NotBeNull();
    response.Value.Should().BeOfType<ReadOnlyCollection<GetCommonFriendsResponse>>();
    response.IsT0.Should().BeTrue();

    scenarioContext.Add(GetCommonFriendsResponseKey, response.AsT0);
  }

  [Then("the response should have following common friend")]
  public void ThenTheResponseShouldHaveFollowingCommonFriend(Table table)
  {
    var userTable = table.CreateSet<UserTable>().ToList();
    userTable.Should().NotBeNull();
    userTable.Count.Should().BeGreaterThan(0);

    var response = scenarioContext.Get<IReadOnlyCollection<GetCommonFriendsResponse>>(GetCommonFriendsResponseKey);
    response.Should().NotBeNull();
    response.Count.Should().BeGreaterThan(0);
    response.Count.Should().Be(userTable.Count);

    foreach (var userTableRow in userTable)
    {
      response.Should().Contain(r =>
        r.UserId == userTableRow.Id.ToString() &&
        r.FullName == userTableRow.FullName);
    }
  }
}
