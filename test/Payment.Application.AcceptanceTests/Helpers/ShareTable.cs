using TechTalk.SpecFlow.Assist.Attributes;

namespace Payment.Application.AcceptanceTests.Helpers;

public sealed class ShareTable
{
  [TableAliases("ShareId")] public Guid Id { get; set; }

  [TableAliases("WalletId")] public Guid WalletId { get; set; }

  [TableAliases("ContributorId")] public Guid ContributorId { get; set; }

  [TableAliases("Currency")] public required string Currency { get; set; }

  [TableAliases("Amount")] public decimal Amount { get; set; }

  [TableAliases("Country")] public string? Country { get; set; }
}
