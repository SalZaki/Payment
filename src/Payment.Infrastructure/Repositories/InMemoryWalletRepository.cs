using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Linq.Expressions;
using Ardalis.GuardClauses;
using Payment.Application.Wallets.Repositories;
using Payment.Domain.Entities;
using Payment.Domain.ValueObjects;

namespace Payment.Infrastructure.Repositories;

public class InMemoryWalletRepository : IWalletRepository
{
  private readonly ConcurrentDictionary<Guid, Wallet> _wallets = new();

  public Task<Wallet> AddAsync(Wallet wallet, CancellationToken cancellationToken = default)
  {
    Guard.Against.Null(wallet, nameof(wallet));

    _wallets.TryAdd<Guid, Wallet>(wallet.Id, wallet);

    return Task.FromResult(wallet);
  }

  public Task<Wallet> UpdateAsync(Wallet wallet, CancellationToken cancellationToken = default)
  {
    Guard.Against.Null(wallet, nameof(wallet));

    if (_wallets.ContainsKey(wallet.Id))
    {
      _wallets[wallet.Id] = wallet;
    }

    return Task.FromResult(wallet);
  }

  public Task DeleteAsync(Expression<Func<Wallet, bool>> predicate, CancellationToken cancellationToken = default)
  {
    Guard.Against.Null(predicate, nameof(predicate));

    var result = _wallets
      .Select(x => x.Value)
      .Where(predicate.Compile());

    foreach (var wallet in result)
    {
      _wallets.Remove(wallet.Id, out _);
    }

    return Task.CompletedTask;
  }

  public Task DeleteAsync(Wallet wallet, CancellationToken cancellationToken = default)
  {
    Guard.Against.Null(wallet, nameof(wallet));

    _wallets.Remove(wallet.Id, out _);

    return Task.CompletedTask;
  }

  public Task DeleteByIdAsync(WalletId id, CancellationToken cancellationToken = default)
  {
    Guard.Against.Null(id, nameof(id));

    _wallets.Remove(id, out _);

    return Task.CompletedTask;
  }

  public Task DeleteRangeAsync(IReadOnlyList<Wallet> entities, CancellationToken cancellationToken = default)
  {
    Guard.Against.Null(entities, nameof(entities));

    foreach (var entity in entities)
    {
      _wallets.Remove(entity.Id, out _);
    }

    return Task.CompletedTask;
  }

  public Task<IReadOnlyList<Wallet>> GetAllAsync(CancellationToken cancellationToken = default)
  {
    var result = _wallets
      .Select(x => x.Value)
      .ToImmutableList();

    return Task.FromResult<IReadOnlyList<Wallet>>(result);
  }

  public Task<Wallet> FindByIdAsync(WalletId id, CancellationToken cancellationToken = default)
  {
    Guard.Against.Null(id, nameof(id));

    var result = _wallets.FirstOrDefault(x => x.Key == id).Value;

    return Task.FromResult(result ?? Wallet.NotFound);
  }

  public Task<Wallet> FindOneAsync(Expression<Func<Wallet, bool>> predicate,
    CancellationToken cancellationToken = default)
  {
    Guard.Against.Null(predicate, nameof(predicate));

    var result = _wallets
      .Select(x => x.Value)
      .Where(predicate.Compile())
      .FirstOrDefault();

    return Task.FromResult(result ?? Wallet.NotFound);
  }

  public Task<IReadOnlyList<Wallet>> FindAsync(Expression<Func<Wallet, bool>> predicate,
    CancellationToken cancellationToken = default)
  {
    Guard.Against.Null(predicate, nameof(predicate));

    var result = _wallets
      .Select(x => x.Value)
      .Where(predicate.Compile())
      .ToImmutableList();

    return Task.FromResult<IReadOnlyList<Wallet>>(result);
  }

  public Task CleanUpWalletsAsync(CancellationToken cancellationToken = default)
  {
    foreach (var wallet in _wallets)
    {
      _wallets.TryRemove(wallet);
    }

    return Task.CompletedTask;
  }

  public Task<int> TotalWalletsAsync(CancellationToken cancellationToken = default)
  {
    return Task.FromResult(_wallets.Count);
  }
}
