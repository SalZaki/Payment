namespace Payment.Common.Abstraction.Domain;

public interface IAggregate<T> : IAggregate, IEntity<T>;
