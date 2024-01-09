namespace Payment.Common.Abstraction.Domain;

public interface IBusinessPolicy
{
  bool IsInvalid();

  string Message { get; }
}
