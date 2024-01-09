using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Linq.Expressions;
using Ardalis.GuardClauses;
using Payment.Application.Users.Repositories;
using Payment.Domain.Entities;
using Payment.Domain.ValueObjects;

namespace Payment.Infrastructure.Repositories;

public sealed class InMemoryUserRepository : IUserRepository
{
  private readonly ConcurrentDictionary<Guid, User> _users = new();

  public Task<User> AddAsync(User user, CancellationToken cancellationToken = default)
  {
    Guard.Against.Null(user, nameof(user));

    _users.TryAdd<Guid, User>(user.Id, user);

    return Task.FromResult(user);
  }

  public Task<User> UpdateAsync(User user, CancellationToken cancellationToken = default)
  {
    Guard.Against.Null(user, nameof(user));

    if (_users.ContainsKey(user.Id))
    {
      _users[user.Id] = user;
    }

    return Task.FromResult(user);
  }

  public Task<IReadOnlyList<User>> UpdateAsync(Expression<Func<User, bool>> predicate,
    CancellationToken cancellationToken = default)
  {
    Guard.Against.Null(predicate, nameof(predicate));

    var result = _users
      .Select(x => x.Value)
      .Where(predicate.Compile())
      .ToList();

    foreach (var wallet in result)
    {
      _users[wallet.Id] = wallet;
    }

    return Task.FromResult<IReadOnlyList<User>>(result.ToImmutableList());
  }

  public Task DeleteAsync(Expression<Func<User, bool>> predicate, CancellationToken cancellationToken = default)
  {
    Guard.Against.Null(predicate, nameof(predicate));

    var result = _users
      .Select(x => x.Value)
      .Where(predicate.Compile());

    foreach (var user in result)
    {
      _users.Remove(user.Id, out _);
    }

    return Task.CompletedTask;
  }

  public Task DeleteAsync(User user, CancellationToken cancellationToken = default)
  {
    Guard.Against.Null(user, nameof(user));

    _users.Remove(user.Id, out _);

    return Task.CompletedTask;
  }

  public Task DeleteByIdAsync(UserId id, CancellationToken cancellationToken = default)
  {
    Guard.Against.Null(id, nameof(id));

    _users.Remove(id, out _);

    return Task.CompletedTask;
  }

  public Task DeleteRangeAsync(IReadOnlyList<User> entities, CancellationToken cancellationToken = default)
  {
    Guard.Against.Null(entities, nameof(entities));

    foreach (var entity in entities)
    {
      _users.Remove(entity.Id, out _);
    }

    return Task.CompletedTask;
  }

  public Task<IReadOnlyList<User>> GetAllAsync(CancellationToken cancellationToken = default)
  {
    var result = _users
      .Select(x => x.Value)
      .ToImmutableList();

    return Task.FromResult<IReadOnlyList<User>>(result);
  }

  public Task<User> FindByIdAsync(UserId id, CancellationToken cancellationToken = default)
  {
    Guard.Against.Null(id, nameof(id));

    var result = _users.FirstOrDefault(x => x.Key == id).Value;

    return Task.FromResult(result ?? User.NotFound);
  }

  public Task<User> FindOneAsync(Expression<Func<User, bool>> predicate, CancellationToken cancellationToken = default)
  {
    Guard.Against.Null(predicate, nameof(predicate));

    var result = _users
      .Select(x => x.Value)
      .Where(predicate.Compile())
      .FirstOrDefault();

    return Task.FromResult(result ?? User.NotFound);
  }

  public Task<IReadOnlyList<User>> FindAsync(Expression<Func<User, bool>> predicate,
    CancellationToken cancellationToken = default)
  {
    Guard.Against.Null(predicate, nameof(predicate));

    var result = _users
      .Select(x => x.Value)
      .Where(predicate.Compile())
      .ToImmutableList();

    return Task.FromResult<IReadOnlyList<User>>(result);
  }

  public Task CleanUpUsersAsync(CancellationToken cancellationToken = default)
  {
    _users.Clear();
    return Task.CompletedTask;
  }

  public Task<int> TotalUsersAsync(CancellationToken cancellationToken = default)
  {
    return Task.FromResult(_users.Count);
  }
}
