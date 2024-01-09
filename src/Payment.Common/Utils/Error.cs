namespace Payment.Common.Utils;

public sealed record Error(string Code, string Message)
{
  public static implicit operator string(Error error) => error.Code;

  public override int GetHashCode() => HashCode.Combine(Code, Message);

  public static Error Empty => new(string.Empty, string.Empty);

  public bool Equals(Error? other)
  {
    if (other is null) return false;

    return Code == other.Code && Message == other.Message;
  }
}
