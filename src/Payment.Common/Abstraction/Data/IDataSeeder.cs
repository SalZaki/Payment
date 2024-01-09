namespace Payment.Common.Abstraction.Data;

public interface IDataSeeder
{
  Task SeedAsync(CancellationToken cancellationToken = default);
}
