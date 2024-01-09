namespace Payment.Domain.Exceptions;

public sealed class CurrencyMismatchException() : ArgumentException("Both operands must have the same currency");
