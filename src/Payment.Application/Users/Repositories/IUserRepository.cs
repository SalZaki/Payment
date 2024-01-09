using Payment.Common.Abstraction.Data;
using Payment.Domain.Entities;
using Payment.Domain.ValueObjects;

namespace Payment.Application.Users.Repositories;

public interface IUserRepository : IRepository<User, UserId>
{
  Task CleanUpUsersAsync(CancellationToken cancellationToken = default);

  Task<int> TotalUsersAsync(CancellationToken cancellationToken = default);
}
