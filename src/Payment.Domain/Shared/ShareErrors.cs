using Payment.Common.Utils;

namespace Payment.Domain.Shared;

public static class ShareErrors
{
  public static Func<string, Error> InvalidShareId => shareId =>
    new Error("Share.InvalidShareId", $"The share id {shareId} is invalid.");
}
