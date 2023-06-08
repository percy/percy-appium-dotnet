using System;
namespace PercyIO.Appium
{
  public static class ObjectExtensions
  {
    public static bool IsTrue(this Object obj)
    {
      String? str = obj?.ToString()?.ToLower();
      return str == "true";
    }

    public static bool IsFalse(this Object obj)
    {
      String? str = obj?.ToString()?.ToLower();
      return str == "false";
    }
  }
}
