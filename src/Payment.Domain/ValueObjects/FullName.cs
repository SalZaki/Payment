using Payment.Domain.Exceptions;

namespace Payment.Domain.ValueObjects;

public sealed record FullName
{
  private const int MaxLength = 100;

  private const int MinLength = 2;

  public string Value { get; }

  private FullName(string value)
  {
    if (string.IsNullOrWhiteSpace(value))
    {
      throw new InvalidFullNameException(value);
    }

    if (string.IsNullOrWhiteSpace(value) || value.Length is > MaxLength or < MinLength)
    {
      throw new InvalidFullNameFormatException(value);
    }

    Value = value.Trim();
  }

  public static FullName Create(string value)
  {
    return new FullName(value);
  }

  public override string ToString()
  {
    return Value;
  }

  public static implicit operator string(FullName fullName) => fullName.Value;
}
