using TechTalk.SpecFlow.Assist.Attributes;

namespace Payment.Application.AcceptanceTests.Helpers;

public sealed class FriendshipTable
{
  [TableAliases("UserId")] public required Guid UserId { get; set; }

  [TableAliases("FriendId")] public required Guid FriendId { get; set; }
}
