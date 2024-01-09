namespace Payment.Common.Abstraction.Domain;

public abstract record Aggregate<TId>(TId Id) : Entity<TId>(Id), IAggregate<TId>;
