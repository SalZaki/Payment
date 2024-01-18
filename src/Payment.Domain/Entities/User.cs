using System.Collections.Immutable;
using Ardalis.GuardClauses;
using Payment.Common.Abstraction.Domain;
using Payment.Domain.Exceptions;
using Payment.Domain.Policies;
using Payment.Domain.ValueObjects;

namespace Payment.Domain.Entities;

public sealed record User : Aggregate<UserId>, IComparable<User>, IComparable
{
  private readonly List<Friendship> _friendships = new();

  public FullName FullName { get; }

  public ImmutableHashSet<Friendship> Friendships => _friendships.ToImmutableHashSet();

  public static readonly User NotFound = Create(UserId.Create(Guid.Empty), FullName.Create(nameof(NotFound)));

  private User(UserId userId, FullName fullName) : base(userId)
  {
    FullName = fullName;
  }

  public static User Create(
    UserId userId,
    FullName fullName,
    string? createdBy = "System",
    DateTime? createdOn = null)
  {
    Guard.Against.Null(userId, nameof(userId), "UserId can not be null.");
    Guard.Against.Null(fullName, nameof(fullName), "FullName can not be null.");

    return new User(userId, fullName)
    {
      CreatedBy = createdBy,
      CreatedOn = createdOn ?? DateTime.UtcNow
    };
  }

  public void AddFriend(User friend)
  {
    Guard.Against.Null(friend, nameof(friend), "Friend can not be null.");

    CheckPolicy(new UserCannotAddItselfAsAFriend(Id, friend.Id));

    if (_friendships.Any(x => x.Friend == friend))
    {
      throw new FriendAlreadyExistsException(friend.Id.ToString());
    }

    _friendships.Add(Friendship.Create(FriendId.Create(Guid.NewGuid()), Id, friend));
  }

  public void RemoveFriend(User friend)
  {
    Guard.Against.Null(friend, nameof(friend), "Friend can not be null.");

    var friendship = _friendships.FirstOrDefault(x => x.Friend == friend);
    _friendships.Remove(friendship!);
  }

  public IReadOnlyList<User> GetConnectionList(User friend, int maxLevel)
  {
    var queue = new Queue<User>();
    var parentMap = new Dictionary<UserId, User?>();
    var levelMap = new Dictionary<User, int>();

    queue.Enqueue(this);
    parentMap[Id] = null;
    levelMap[this] = 0;

    while (queue.Count > 0)
    {
      var current = queue.Dequeue();
      var currentLevel = levelMap[current];

      if (current == friend)
      {
        return ReconstructPath(parentMap, friend);
      }

      if (currentLevel >= maxLevel) continue;

      foreach (var friendship in current.Friendships.Where(friendship => !parentMap.ContainsKey(friendship.Friend.Id)))
      {
        queue.Enqueue(friendship.Friend);
        parentMap[friendship.Friend.Id] = current;
        levelMap[friendship.Friend] = currentLevel + 1;
      }
    }

    // No connection found within max level
    return new List<User>();
  }

  public IReadOnlyList<User> GetCommonFriends(User user)
  {
    Guard.Against.Null(user, nameof(user), "User can not be null.");

    if (_friendships.Count == 0 || user.Friendships.Count == 0)
    {
      return new List<User>().AsReadOnly();
    }

    var intersect = user.Friendships.Select(x => x.Friend).ToList();
    var commonFriends = intersect.Intersect(_friendships.Select(x => x.Friend).ToList());

    return commonFriends.ToList().AsReadOnly();
  }

  public int CompareTo(User? other)
  {
    if (other is null) return 1;

    return Id == other.Id ? 0 : 1;
  }

  public int CompareTo(object? obj)
  {
    if (obj is null) return 1;

    return obj is not User user ? 1 : CompareTo(user);
  }

  public override int GetHashCode()
  {
    return HashCode.Combine(Id, FullName);
  }

  private static List<User> ReconstructPath(IReadOnlyDictionary<UserId, User?> parentMap, User friend)
  {
    var path = new List<User>();
    var current = friend;

    while (current != null)
    {
      path.Add(current);
      current = parentMap[current.Id];
    }

    path.Reverse();

    return path;
  }
}
