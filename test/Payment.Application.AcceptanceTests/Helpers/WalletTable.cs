using TechTalk.SpecFlow.Assist.Attributes;

namespace Payment.Application.AcceptanceTests.Helpers;

public sealed class WalletTable
{
  [TableAliases("WalletId")] public required Guid Id { get; set; }

  [TableAliases("UserId")] public required Guid UserId { get; set; }

  [TableAliases("ContributorId")] public required Guid ContributorId { get; set; }

  [TableAliases("ShareCount")] public required int ShareCount { get; set; }

  [TableAliases("Currency")] public required string Currency { get; set; }

  public required decimal Amount { get; set; }

  public required decimal TotalSharesAmount { get; set; }

  [TableAliases("CreatedBy")] public string? CreatedBy { get; set; }

  [TableAliases("CreatedOn")] public DateTime? CreatedOn { get; set; }
}
