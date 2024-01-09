using Ardalis.GuardClauses;
using Payment.Domain.Exceptions;

namespace Payment.Domain.Extensions;

public static class GuardExtensions
{
  public static decimal MaxAmount(
    this IGuardClause guardClause,
    decimal amount)
  {
    if (amount > 1000000)
    {
      throw new MaxAmountException(amount);
    }

    return amount;
  }

  public static decimal Negative(
    this IGuardClause guardClause,
    decimal input)
  {
    return Negative(guardClause, input, new NegativeAmountException(input));
  }

  public static string InvalidFullName(
    this IGuardClause guardClause,
    string fullName,
    Exception exception)
  {
    if (string.IsNullOrWhiteSpace(fullName))
    {
      throw exception;
    }

    return fullName.Length switch
    {
      < 2 => throw exception,
      > 100 => throw exception,
      _ => fullName
    };
  }

  public static decimal NegativeOrZero(
    this IGuardClause guardClause,
    decimal input)
  {
    return Negative(guardClause, input, new NegativeOrZeroAmountException(input));
  }

  private static T Negative<T>(
    this IGuardClause guardClause,
    T input,
    Exception exception)
    where T : struct, IComparable
  {

    if (input.CompareTo(default(T)) < 0)
    {
      throw exception;
    }

    return input;
  }

  private static T NegativeOrZero<T>(
    this IGuardClause guardClause,
    T input,
    Exception exception)
    where T : struct, IComparable
  {
    if (input.CompareTo(default(T)) <= 0)
    {
      throw exception;
    }

    return input;
  }
}
