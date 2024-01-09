using TechTalk.SpecFlow.Assist.Attributes;

namespace Payment.Application.AcceptanceTests.Helpers;

public sealed class UserTable
{
  [TableAliases("UserId")] public required Guid Id { get; set; }

  [TableAliases("FullName")] public required string FullName { get; set; }

  [TableAliases("CreatedBy")] public string? CreatedBy { get; set; }

  [TableAliases("CreatedOn")] public DateTime? CreatedOn { get; set; }
}
