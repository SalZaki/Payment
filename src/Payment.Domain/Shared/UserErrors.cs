using Payment.Common.Utils;

namespace Payment.Domain.Shared;

public static class UserErrors
{
  public static Func<string, Error> FriendAlreadyExists => id =>
    new Error("User.FriendAlreadyExists", $"The user with id '{id}' is already a friend.");

  public static Func<string, Error> InvalidFriendRequest => id =>
    new Error("User.InvalidFriendRequest", $"User with id '{id}' cannot add itself as a friend.");

  public static Func<string, Error> InvalidFullName => value =>
    new Error("User.InvalidFullName", $"FullName '{value}' is invalid.");

  public static Func<string, Error> InvalidFullNameFormat => value =>
    new Error("User.InvalidFullNameFormat", $"FullName '{value}' format is invalid.");
}
