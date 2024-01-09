using System.Collections.ObjectModel;
using FluentAssertions;
using OneOf;
using Payment.Application.AcceptanceTests.Contexts;
using Payment.Application.AcceptanceTests.Helpers;
using Payment.Application.Users.Features.GetConnectionList;
using Payment.Common.Abstraction.Models;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace Payment.Application.AcceptanceTests.Steps.Users;

[Binding]
[Scope(Feature = "Get Connection List")]
public class GetConnectionListHappyPaths(ScenarioContext scenarioContext, TestContext testContext)
{
  private const string UserId1Key = "UserId1";
  private const string UserId2Key = "UserId2";
  private const string MaxLevelKey = "MaxLevel";
  private const string ResponseKey = "Response";
  private const string GetConnectionListResponseKey = "GetConnectionListResponse";

  [Given("following users in the system")]
  public async Task GivenFollowingUsersInTheSystem(Table table)
  {
    var userTable = table.CreateSet<UserTable>().ToList();
    userTable.Should().NotBeNull();
    userTable.Count.Should().BeGreaterThan(0);
    await testContext.SeedUsers(userTable);
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

  [Given("max level is (.*)")]
  public void GivenMaxLevelIs(int maxLevel)
  {
    scenarioContext.Add(MaxLevelKey, maxLevel);
  }

  [When("I submit the request to get connection list")]
  public async Task WhenISubmitTheRequestToGetConnectionList()
  {
    var userId1 = scenarioContext.Get<string>(UserId1Key);
    var userId2 = scenarioContext.Get<string>(UserId2Key);
    var maxLevel = scenarioContext.Get<int>(MaxLevelKey);

    var query = new GetConnectionListQuery
    {
      UserId1 = userId1,
      UserId2 = userId2,
      MaxLevel = maxLevel
    };

    var response = await testContext.UserService.GetConnectionListAsync(query, CancellationToken.None);

    scenarioContext.Add(ResponseKey, response);
  }

  [Then("the response should be successful")]
  public void ThenTheResponseShouldBeSuccessful()
  {
    var response = scenarioContext.Get<OneOf<IReadOnlyCollection<GetConnectionListResponse>, ValidationErrorResponse, ErrorResponse>>(ResponseKey);
    response.Should().NotBeNull();
    response.Value.Should().BeOfType<ReadOnlyCollection<GetConnectionListResponse>>();
    response.IsT0.Should().BeTrue();

    scenarioContext.Add(GetConnectionListResponseKey, response.AsT0);
  }

  [Then("the response should have following connected list")]
  public void ThenTheResponseShouldHaveFollowingConnectedList(Table table)
  {
    var userTable = table.CreateSet<UserTable>().ToList();
    userTable.Should().NotBeNull();
    userTable.Count.Should().BeGreaterThan(0);

    var response = scenarioContext.Get<IReadOnlyCollection<GetConnectionListResponse>>(GetConnectionListResponseKey);
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

  [Given("the user1 has user2 as friend in the system")]
  public async Task GivenTheUser1HasUser2AsFriendInTheSystem(Table table)
  {
    var friendshipTable = table.CreateSet<FriendshipTable>().ToList();
    friendshipTable.Should().NotBeNull();
    friendshipTable.Count.Should().BeGreaterThan(0);
    await testContext.SeedFriends(friendshipTable);
  }

  [Given("the user2 has user1 as friend in the system")]
  public async Task GivenTheUser2HasUser1AsFriendInTheSystem(Table table)
  {
    var friendshipTable = table.CreateSet<FriendshipTable>().ToList();
    friendshipTable.Should().NotBeNull();
    friendshipTable.Count.Should().BeGreaterThan(0);
    await testContext.SeedFriends(friendshipTable);
  }

  [Given("user1 has user3 as friend in the system")]
  public async Task GivenUser1HasUser2AsFriendInTheSystem(Table table)
  {
    var friendshipTable = table.CreateSet<FriendshipTable>().ToList();
    friendshipTable.Should().NotBeNull();
    friendshipTable.Count.Should().BeGreaterThan(0);
    await testContext.SeedFriends(friendshipTable);
  }

  [Given("user3 has user4 as friend in the system")]
  public async Task GivenUser3HasUser4AsFriendInTheSystem(Table table)
  {
    var friendshipTable = table.CreateSet<FriendshipTable>().ToList();
    friendshipTable.Should().NotBeNull();
    friendshipTable.Count.Should().BeGreaterThan(0);
    await testContext.SeedFriends(friendshipTable);
  }

  [Given("user4 has user2 as friend in the system")]
  public async Task GivenUser4HasUser2AsFriendInTheSystem(Table table)
  {
    var friendshipTable = table.CreateSet<FriendshipTable>().ToList();
    friendshipTable.Should().NotBeNull();
    friendshipTable.Count.Should().BeGreaterThan(0);
    await testContext.SeedFriends(friendshipTable);
  }

  [Then("the response should have empty connected list")]
  public void ThenTheResponseShouldHaveEmptyConnectedList()
  {
    var response = scenarioContext.Get<IReadOnlyCollection<GetConnectionListResponse>>(GetConnectionListResponseKey);
    response.Should().NotBeNull();
    response.Count.Should().Be(0);
  }
}
