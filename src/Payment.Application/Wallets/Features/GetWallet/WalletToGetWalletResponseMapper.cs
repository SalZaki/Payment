using System.Collections.ObjectModel;
using Payment.Common.Mappers;
using Payment.Domain.Entities;

namespace Payment.Application.Wallets.Features.GetWallet;

public sealed class WalletToGetWalletResponseMapper : IMapper<Wallet, GetWalletResponse>
{
  public GetWalletResponse Map(Wallet? source)
  {
    if (source == null) return null!;

    return new GetWalletResponse
    {
      WalletId = source.Id.ToString(),
      UserId = source.UserId.ToString(),
      Amount = source.Amount.InMajorUnits,
      Currency = source.Amount.Currency.Code,
      Shares = MapShare(source.Shares),
      TotalSharesAmount = source.TotalSharesAmount.AsReadOnly(),
      CreatedBy = source.CreatedBy,
      CreatedOn = source.CreatedOn,
    };
  }

  private static ReadOnlyCollection<GetWalletShareResponse> MapShare(IEnumerable<Share> source)
  {
    return source.Select(x =>
        new GetWalletShareResponse
        {
          ShareId = x.Id.ToString(),
          WalletId = x.WalletId.ToString(),
          ContributorId = x.ContributorId.ToString(),
          Amount = x.Amount.InMajorUnits,
          Currency = x.Amount.Currency.Code,
          CreatedBy = x.CreatedBy,
          CreatedOn = x.CreatedOn
        })
      .ToList()
      .AsReadOnly();
  }
}
