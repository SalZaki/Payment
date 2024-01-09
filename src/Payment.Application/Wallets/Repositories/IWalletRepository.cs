using Payment.Common.Abstraction.Data;
using Payment.Domain.Entities;
using Payment.Domain.ValueObjects;

namespace Payment.Application.Wallets.Repositories;

public interface IWalletRepository : IRepository<Wallet, WalletId>
{
  Task CleanUpWalletsAsync(CancellationToken cancellationToken = default);

  Task<int> TotalWalletsAsync(CancellationToken cancellationToken = default);
}
