using Ardalis.GuardClauses;
using Payment.Common.Abstraction.Domain;
using Payment.Domain.Exceptions;
using Payment.Domain.Policies;
using Payment.Domain.ValueObjects;

namespace Payment.Domain.Entities;

public sealed record User : Aggregate<UserId>, IComparable<User>, IComparable
{
  public FullName FullName { get; }

  public HashSet<Friendship> Friendships { get; }

  public static readonly User NotFound = Create(UserId.Create(Guid.Empty), FullName.Create(nameof(NotFound)));

  private User(UserId userId, FullName fullName) : base(userId)
  {
    FullName = fullName;
    Friendships = new HashSet<Friendship>();
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

    if (Friendships.Any(x => x.Friend == friend))
    {
      throw new FriendAlreadyExistsException(friend.Id.ToString());
    }

    Friendships.Add(Friendship.Create(FriendId.Create(Guid.NewGuid()), Id, friend));

    // Assuming here that all friendships are bidirectional
    friend.Friendships.Add(Friendship.Create(FriendId.Create(Guid.NewGuid()), friend.Id, this));
  }

  public void RemoveFriend(User friend)
  {
    Guard.Against.Null(friend, nameof(friend), "Friend can not be null.");

    Friendships.RemoveWhere(x => x.Friend == friend);
  }

  public void RemoveFriends(IEnumerable<Friendship> friends)
  {
    Friendships.RemoveWhere(f => friends.Any(x => x.Friend == f.Friend));
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

    if (Friendships.Count == 0 || user.Friendships.Count == 0)
    {
      return new List<User>().AsReadOnly();
    }

    var intersect = user.Friendships.Select(x => x.Friend).ToList();
    var commonFriends = intersect.Intersect(Friendships.Select(x => x.Friend).ToList());

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
