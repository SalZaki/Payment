namespace Payment.Application.UnitTests.Helpers;

public static class ExceptionHelper
{
  public static Exception? CreateExceptionBy(Type type, string message)
  {
    if (!type.IsValueType && typeof(Exception).IsAssignableFrom(type))
    {
      return Activator.CreateInstance(type, message) as Exception;
    }

    return null;
  }
}
