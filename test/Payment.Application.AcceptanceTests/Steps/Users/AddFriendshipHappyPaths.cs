using FluentAssertions;
using Payment.Application.AcceptanceTests.Contexts;
using Payment.Application.Users.Features.AddFriendship;
using Payment.Common.Abstraction.Models;
using Payment.Domain.Entities;
using Payment.Domain.ValueObjects;
using TechTalk.SpecFlow;
using OneOf;
using Payment.Application.AcceptanceTests.Helpers;
using TechTalk.SpecFlow.Assist;

namespace Payment.Application.AcceptanceTests.Steps.Users;

[Binding]
[Scope(Feature = "Add Friendship")]
public class AddFriendshipHappyPaths(ScenarioContext scenarioContext, TestContext testContext)
{
  private const string UserIdKey = "UserId";
  private const string FriendIdKey = "FriendId";
  private const string ResponseKey = "Response";
  private const string AddFriendshipResponseKey = "AddFriendshipResponse";

  [Given("following users in the system")]
  public async Task GivenFollowingUsersInTheSystem(Table table)
  {
    var userTable = table.CreateSet<UserTable>().ToList();
    userTable.Should().NotBeNull();
    userTable.Count.Should().BeGreaterThan(0);
    await testContext.SeedUsers(userTable);
  }

  [Given("a user id (.*)")]
  public void GivenAUserId(Guid userId)
  {
    scenarioContext.Add(UserIdKey, userId);
  }

  [Given("a friend id (.*)")]
  public void GivenAFriendId(Guid friendId)
  {
    scenarioContext.Add(FriendIdKey, friendId);
  }

  [When("I submit the request to add friendship")]
  public async Task WhenISubmitTheRequestToAddFriendship()
  {
    var userId = scenarioContext.Get<Guid>(UserIdKey);
    var friendId = scenarioContext.Get<Guid>(FriendIdKey);

    var request = new AddFriendshipCommand
    {
      UserId = userId.ToString(),
      FriendId = friendId.ToString()
    };

    var response = await testContext.UserService.AddFriendAsync(request, CancellationToken.None);

    scenarioContext.Add(ResponseKey, response);
  }

  [Then("the response should be successful")]
  public void ThenTheResponseShouldBeSuccessful()
  {
    var response = scenarioContext.Get<OneOf<AddFriendshipResponse, ValidationErrorResponse, ErrorResponse>>(ResponseKey);
    response.Should().NotBeNull();
    response.Value.Should().BeOfType<AddFriendshipResponse>();
    response.IsT0.Should().BeTrue();

    scenarioContext.Add(AddFriendshipResponseKey, response.AsT0);
  }

  [Then("the response should have a newly created friend id")]
  public void ThenTheResponseShouldHaveANewlyCreatedFriendId()
  {
    var friendId = scenarioContext.Get<Guid>(FriendIdKey);
    var response = scenarioContext.Get<AddFriendshipResponse>(AddFriendshipResponseKey);

    response.FriendId.Should().NotBeNullOrEmpty();
    response.FriendId.Should().Be(friendId.ToString());
  }

  [Then("the friend should exit in the friendships list")]
  public async Task ThenTheFriendShouldExitInTheFriendshipsList()
  {
    var userId = UserId.Create(scenarioContext.Get<Guid>(UserIdKey));
    var friendId = UserId.Create(scenarioContext.Get<Guid>(FriendIdKey));

    var user = await testContext.UserRepository.FindByIdAsync(userId, CancellationToken.None);
    var friend = await testContext.UserRepository.FindByIdAsync(friendId, CancellationToken.None);

    user.Should().NotBe(User.NotFound);
    friend.Should().NotBe(User.NotFound);

    user.Friendships.Should().NotBeNull();
    user.Friendships.Count.Should().BeGreaterThan(0);
    user.Friendships.Any(x => x.UserId == userId).Should().BeTrue();
    user.Friendships.Any(x => x.Friend == friend).Should().BeTrue();
  }

  [Then("both should be mutual friends")]
  public async Task ThenBothShouldBeMutualFriends()
  {
    var userId = UserId.Create(scenarioContext.Get<Guid>(UserIdKey));
    var friendId = UserId.Create(scenarioContext.Get<Guid>(FriendIdKey));

    var user = await testContext.UserRepository.FindByIdAsync(userId, CancellationToken.None);
    var friend = await testContext.UserRepository.FindByIdAsync(friendId, CancellationToken.None);

    user.Should().NotBe(User.NotFound);
    friend.Should().NotBe(User.NotFound);

    user.Friendships.Should().NotBeNull();
    user.Friendships.Count.Should().BeGreaterThan(0);
    user.Friendships.Any(x => x.UserId == userId).Should().BeTrue();
    user.Friendships.Any(x => x.Friend == friend).Should().BeTrue();

    // Mutual friendship
    friend.Friendships.Should().NotBeNull();
    friend.Friendships.Count.Should().BeGreaterThan(0);
    friend.Friendships.Any(x => x.UserId == friendId).Should().BeTrue();
    friend.Friendships.Any(x => x.Friend == user).Should().BeTrue();
  }
}
