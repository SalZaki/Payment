using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Payment.Domain.Entities;
using Payment.Domain.ValueObjects;
using Xunit;

namespace Payment.Domain.UnitTests.TestData;

public static class UserTestData
{
  private static readonly Fixture Fixture;
  private static readonly DateTime CreatedOn;
  private static string CreatedBy;

  static UserTestData()
  {
    Fixture = new Fixture();
    Fixture.Customizations.Add(new RandomNumericSequenceGenerator(0, 100));
    Fixture.Customize(new AutoNSubstituteCustomization {ConfigureMembers = true});

    CreatedOn = DateTime.UtcNow;
    CreatedBy = "UserTest-->{0}";
  }

  public static class AddFriend
  {
    static AddFriend()
    {
      CreatedBy = string.Format(CreatedBy, nameof(AddFriend));
    }

    public sealed class ValidInputUserAndFriend : TheoryData<User, User, string, DateTime>
    {
      public ValidInputUserAndFriend()
      {
        var user = Fixture.Create<User>();
        var friend = Fixture.Create<User>();

        Add(user, friend, CreatedBy, CreatedOn);
      }
    }

    public sealed class InValidInputUserAndFriend : TheoryData<User, User>
    {
      public InValidInputUserAndFriend()
      {
        var user = Fixture.Create<User>();
        var friend = Fixture.Create<User>();
        user.AddFriend(friend);

        Add(user, friend);
      }
    }
  }

  public static class RemoveFriend
  {
    public sealed class ValidInputUserAndFriend : TheoryData<User, User>
    {
      public ValidInputUserAndFriend()
      {
        var friend = Fixture.Create<User>();
        var user = Fixture.Create<User>();
        user.AddFriend(friend);

        Add(user, friend);
      }
    }
  }

  public static class GetCommonFriends
  {
    public sealed class ValidInputUser1AndUser2 : TheoryData<User, User, IReadOnlyList<User>>
    {
      public ValidInputUser1AndUser2()
      {
        var commonFriend = User.Create(Fixture.Create<UserId>(), FullName.Create("John Doe"));

        var user1 = Fixture.Create<User>();
        user1.AddFriend(commonFriend);

        var user2 = Fixture.Create<User>();
        user2.AddFriend(commonFriend);

        Add(user1, user2, new[] {commonFriend});
      }
    }
  }
}
