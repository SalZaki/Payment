using AutoFixture;
using AutoFixture.Idioms;
using FluentAssertions;
using Payment.Domain.Entities;
using Payment.Domain.Exceptions;
using Payment.Domain.UnitTests.TestData;
using Payment.Domain.UnitTests.ValueObjects;
using Payment.Domain.ValueObjects;
using Xunit;

namespace Payment.Domain.UnitTests.Entities;

/// <summary>
/// Could filter by `dotnet test --filter "Category=User"` in running tests in command line
/// </summary>
[Trait("Category", "User")]
public sealed class UserTests : BaseTest
{
  [Theory(DisplayName = "Constructor --> with guard clauses assertion")]
  [Trait("Category", "User.Constructor")]
  [UserAutoNSubstituteData]
  public void ShouldHaveConstructorGuardClauses(GuardClauseAssertion assertion)
  {
    // act & assert
    assertion.Verify(typeof(User).GetConstructors());
  }

  [Fact(DisplayName = "Create --> with null user id then should fail")]
  [Trait("Category", "User.Create")]
  public void Create_WithNullUserId_ShouldThrowException()
  {
    // arrange
    var userId = null as UserId;
    var fullname = Fixture.Create<FullName>();

    // act
    var act = () => { User.Create(userId!, fullname); };

    // assert
    act.Should()
      .Throw<ArgumentNullException>()
      .Where(x => x.Message.StartsWith("UserId can not be null."))
      .WithParameterName(nameof(userId));
  }

  [Fact(DisplayName = "Create --> with null fullName then should fail")]
  [Trait("Category", "User.Create")]
  public void Create_WithNullFullname_ShouldThrowException()
  {
    // arrange
    var userId = Fixture.Create<UserId>();
    var fullName = null as FullName;

    // act
    var act = () => { User.Create(userId, fullName!); };

    // assert
    act.Should()
      .Throw<ArgumentNullException>()
      .Where(x => x.Message.StartsWith("FullName can not be null."))
      .WithParameterName(nameof(fullName));
  }

  [Fact(DisplayName = "Create --> with fullName then should create")]
  [Trait("Category", "User.Create")]
  public void Create_WithValidInputs_ShouldSucceed()
  {
    // arrange
    var userId = Fixture.Create<UserId>();
    var fullName = Fixture.Create<FullName>();

    // act
    var user = User.Create(userId, fullName);

    // assert
    user.Should().NotBeNull();
    user.Id.Should().NotBeNull();
    user.Id.Value.Should().Be(userId.Value);
    user.FullName.Should().NotBeNull();
    user.FullName.Value.Should().Be(fullName.Value);
  }

  [Fact(DisplayName = "AddFriend --> with null friend then should fail")]
  [Trait("Category", "User.AddFriend")]
  public void AddFriend_WithNullFriend_ShouldThrowException()
  {
    // arrange
    var user = Fixture.Create<User>();
    var friend = null as User;

    // act
    var act = () => { user.AddFriend(friend!); };

    // assert
    act.Should()
      .Throw<ArgumentNullException>().Where(x => x.Message.StartsWith("Friend can not be null."))
      .WithParameterName(nameof(friend));
  }

  [Fact(DisplayName = "AddFriend --> with valid input then should add")]
  [Trait("Category", "User.AddFriend")]
  public void AddFriend_WithValidInput_ShouldSucceed()
  {
    // arrange
    var user = Fixture.Create<User>();
    var friend = Fixture.Create<User>();

    // act
    user.AddFriend(friend);

    // assert
    user.Friendships.Should().NotBeNull();
    user.Friendships.Should().HaveCountGreaterThan(0);
    user.Friendships.Any(x => x.Friend.Id == friend.Id).Should().BeTrue();
  }

  [Theory(DisplayName = "AddFriend --> with friend already exist then should fail")]
  [Trait("Category", "User.AddFriend")]
  [ClassData(typeof(UserTestData.AddFriend.InValidInputUserAndFriend))]
  public void AddFriend_WithFriendAlreadyExist_ShouldThrowException(User user, User friend)
  {
    // arrange & act
    var act = () => { user.AddFriend(friend); };

    // assert
    act.Should()
      .Throw<FriendAlreadyExistsException>()
      .WithMessage($"The user with id '{friend.Id}' is already a friend.");
  }

  [Theory(DisplayName = "RemoveFriend --> with valid input then should remove")]
  [Trait("Category", "User.RemoveFriend")]
  [ClassData(typeof(UserTestData.RemoveFriend.ValidInputUserAndFriend))]
  public void RemoveFriend_WithValidInput_ShouldSucceed(User user, User friend)
  {
    // act
    user.RemoveFriend(friend);

    // assert
    user.Friendships.Should().NotBeNull();
    user.Friendships.Any(x => x.Friend.Id == friend.Id).Should().BeFalse();
  }

  [Theory(DisplayName = "GetCommonFriends --> with valid input then should return")]
  [Trait("Category", "User.GetCommonFriends")]
  [ClassData(typeof(UserTestData.GetCommonFriends.ValidInputUser1AndUser2))]
  public void GetCommonFriends_WithValidInput_ShouldSucceed(User user1, User user2, IReadOnlyList<User> expectedResponse)
  {
    // acts
    var actualResponse = user1.GetCommonFriends(user2);

    // assert
    actualResponse.Should().NotBeNull();
    actualResponse.Count.Should().Be(expectedResponse.Count);
    actualResponse.SequenceEqual(expectedResponse).Should().BeTrue();
  }
}
