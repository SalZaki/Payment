namespace Payment.Common.Abstraction.Commands;

public interface ICommandHandler<in TCommand, TResult>
  where TCommand : class, ICommand
  where TResult : notnull
{
  Task<TResult> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}
