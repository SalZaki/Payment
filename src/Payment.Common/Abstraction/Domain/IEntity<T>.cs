namespace Payment.Common.Abstraction.Domain;

public interface IEntity<T> : IEntity
{
  public T Id { get; init; }
}
