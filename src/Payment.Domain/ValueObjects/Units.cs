namespace Payment.Domain.ValueObjects;

public sealed record Units : IComparable<Units>, IComparable
{
  public required string Value { get; init; }

  public static Units Major => Create(nameof(Major));

  public static Units Minor => Create(nameof(Minor));

  public static Units None => Create(nameof(None));

  private static Units Create(string value)
  {
    return new Units {Value = value};
  }

  public int CompareTo(Units? other)
  {
    return other is null ? 1 : string.Compare(Value, other.Value, StringComparison.Ordinal);
  }

  public int CompareTo(object? obj)
  {
    if (obj is null) return 1;

    return obj is not Units units ? 1 : CompareTo(units);
  }
}
