using System.Collections.ObjectModel;
using Payment.Common.Mappers;
using Payment.Domain.Entities;

namespace Payment.Application.Users.Features.GetUser;

public class UserToGetUserResponseMapper : IMapper<User, GetUserResponse>
{
  public GetUserResponse Map(User source)
  {
    return new GetUserResponse
    {
      UserId = source.Id.ToString(),
      FullName = source.FullName.ToString(),
      Friendships = source.Friendships.Count != 0 ? MapShare(source.Friendships) : Array.Empty<GetFriendshipResponse>(),
      CreatedBy = source.CreatedBy,
      CreatedOn = source.CreatedOn
    };
  }

  private static ReadOnlyCollection<GetFriendshipResponse> MapShare(IEnumerable<Friendship> source)
  {
    return source.Select(x =>
        new GetFriendshipResponse
        {
          Friend = new GetUserResponse
          {
            UserId = x.Friend.Id.ToString(),
            FullName = x.Friend.FullName
          }
        })
      .ToList()
      .AsReadOnly();
  }
}
