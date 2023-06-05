namespace PercyIO.Appium
{
  public static class ObjectExtensions
  {
    public static bool IsTrue(this object obj)
    {
      string? str = obj?.ToString()?.ToLower();
      return str == "true";
    }

    public static bool IsFalse(this object obj)
    {
      string? str = obj?.ToString()?.ToLower();
      return str == "false";
    }
  }
}
