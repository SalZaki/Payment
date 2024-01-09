using Payment.Application.AcceptanceTests.Helpers;
using Payment.Application.Users.Repositories;
using Payment.Application.Users.Services;
using Payment.Application.Wallets.Repositories;
using Payment.Application.Wallets.Services;
using Payment.Domain.Entities;
using Payment.Domain.ValueObjects;
using Payment.Infrastructure.Repositories;

namespace Payment.Application.AcceptanceTests.Contexts;

public sealed class TestContext
{
  public required IUserService UserService { get; set; }

  public required IWalletService WalletService { get; set; }

  public IUserRepository UserRepository { get; }

  public IWalletRepository WalletRepository { get; }

  private TestContext()
  {
    UserRepository = new InMemoryUserRepository();
    WalletRepository = new InMemoryWalletRepository();
  }

  public async Task SeedUsers(IEnumerable<UserTable> userTable)
  {
    foreach (var user in userTable)
    {
      var userEntity = User.Create(
        UserId.Create(user.Id),
        FullName.Create(user.FullName),
        user.FullName,
        DateTime.UtcNow);
      await UserRepository.AddAsync(userEntity, CancellationToken.None);
    }
  }

  public async Task SeedFriends(IEnumerable<FriendshipTable> friendshipTable)
  {
    foreach (var friendRow in friendshipTable)
    {
      var user = await GetUser(UserId.Create(friendRow.UserId));
      if (user == User.NotFound) continue;

      var friend = await GetUser(UserId.Create(friendRow.FriendId));
      if (friend == User.NotFound) continue;

      var friendship = Friendship.Create(FriendId.Create(Guid.NewGuid()), user.Id, friend);
      user.Friendships.Add(friendship);
      await UserRepository.UpdateAsync(user, CancellationToken.None);
    }
  }

  public async Task SeedWallets(IEnumerable<WalletTable> walletTable)
  {
    foreach (var wallet in walletTable)
    {
      var userEntity = Wallet.Create(
        WalletId.Create(wallet.Id),
        UserId.Create(wallet.UserId),
        Money.Create(wallet.Amount, Currency.Parse(wallet.Currency), Units.Major),
        "TestSystem",
        DateTime.UtcNow);

      await WalletRepository.AddAsync(userEntity, CancellationToken.None);
    }
  }

  public async Task SeedShares(IEnumerable<ShareTable> shareTable)
  {
    var shares = shareTable.Select(share =>
        Share.Create(ShareId.Create(Guid.NewGuid()),
          WalletId.Create(share.WalletId),
          UserId.Create(share.ContributorId),
          Money.Create(share.Amount, Currency.Parse(share.Currency), Units.Major),
          "TestSystem",
          DateTime.UtcNow))
      .ToList()
      .AsReadOnly();

    foreach (var share in shares)
    {
      var wallet = await WalletRepository.FindByIdAsync(share.WalletId, CancellationToken.None);
      if (wallet == Wallet.NotFound) continue;
      wallet.Shares.Add(share);

      await WalletRepository.UpdateAsync(wallet, CancellationToken.None);
    }
  }

  private async Task<User> GetUser(UserId userId)
  {
    return await UserRepository.FindOneAsync(x => x.Id == userId, CancellationToken.None);
  }
}
