using FluentAssertions;
using Payment.Application.AcceptanceTests.Contexts;
using Payment.Application.AcceptanceTests.Helpers;
using Payment.Application.Users.Features.GetUser;
using Payment.Common.Abstraction.Models;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using OneOf;

namespace Payment.Application.AcceptanceTests.Steps.Users;

[Binding]
[Scope(Feature = "Get User")]
public class GetUserHappyPaths(ScenarioContext scenarioContext, TestContext testContext)
{
  private const string GetUserQueryKey = "GetUserQuery";
  private const string ResponseKey = "Response";
  private const string GetUserResponseKey = "GetUserResponse";

  [Given("following users in the system")]
  public async Task GivenFollowingUsersInTheSystem(Table table)
  {
    var userTable = table.CreateSet<UserTable>().ToList();
    userTable.Should().NotBeNull();
    userTable.Count.Should().BeGreaterThan(0);
    await testContext.SeedUsers(userTable);
  }

  [Given("following friendships for the user exist in the system")]
  public async Task GivenFollowingFriendshipsForTheUserExistInTheSystem(Table table)
  {
    var friendshipTable = table.CreateSet<FriendshipTable>().ToList();
    friendshipTable.Should().NotBeNull();
    friendshipTable.Count.Should().BeGreaterThan(0);
    await testContext.SeedFriends(friendshipTable);
  }

  [Given("a user with id (.*)")]
  public void GivenAUserWithId(Guid userId)
  {
    var query = new GetUserQuery
    {
      UserId = userId.ToString()
    };

    scenarioContext.Add(GetUserQueryKey, query);
  }

  [When("I submit the query to get the user")]
  public async Task WhenISubmitTheQueryToGetTheUser()
  {
    var query = scenarioContext.Get<GetUserQuery>(GetUserQueryKey);

    var response = await testContext.UserService.GetUserAsync(query, CancellationToken.None);

    scenarioContext.Add(ResponseKey, response);
  }

  [Then("the response should be successful")]
  public void ThenTheResponseShouldBeSuccessful()
  {
    var response = scenarioContext.Get<OneOf<GetUserResponse, ValidationErrorResponse, ErrorResponse>>(ResponseKey);
    response.Should().NotBeNull();
    response.Value.Should().BeOfType<GetUserResponse>();
    response.IsT0.Should().BeTrue();

    scenarioContext.Add(GetUserResponseKey, response.AsT0);
  }

  [Then("the response should be a user with id (.*)")]
  public void ThenTheResponseShouldBeAUserWithId(Guid userId)
  {
    var response = scenarioContext.Get<GetUserResponse>(GetUserResponseKey);

    response.Should().NotBeNull();
    response.UserId.Should().NotBeNullOrEmpty();
    response.UserId.Should().BeEquivalentTo(userId.ToString());
  }

  [Then(@"the user should have a friend with id (.*) and fullname ""(.*)""")]
  public void ThenTheUserShouldHaveAFriendWithIdAndFullname(Guid friendId, string fullname)
  {
    var response = scenarioContext.Get<GetUserResponse>(GetUserResponseKey);

    response.Should().NotBeNull();
    response.Should().BeOfType<GetUserResponse>();
    response.Friendships.Should().NotBeNull();
    response.Friendships?.Count.Should().BeGreaterThan(0);
    response.Friendships?.Any(x => x.Friend.UserId == friendId.ToString()).Should().BeTrue();
    response.Friendships?.Any(x => x.Friend.FullName == fullname).Should().BeTrue();
  }
}
