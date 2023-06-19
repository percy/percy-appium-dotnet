using System;

namespace PercyIO.Appium
{
  internal class Utils
  {
    public static readonly bool DEBUG = Environment.GetEnvironmentVariable("PERCY_LOGLEVEL") == "debug";
    public static readonly string[] SupportedDriverClassnames = { "OpenQA.Selenium.Appium.Android.AndroidDriver", "OpenQA.Selenium.Appium.iOS.IOSDriver" };
    
    public static Boolean isValidDriverObject(Object obj)
    {
      String type = obj.GetType().ToString();
      foreach (string constant in SupportedDriverClassnames)
      {
        if (type.Contains(constant))
        {
          return true;
        }
      }

      return false;
    }

    public static void Log(String message, String logLevel = "info")
    {
      if (logLevel == "debug" && DEBUG)
      {
        string label = "percy:dotnet";
        LogMessage(message, label, "91m");
      }
      else if (logLevel == "info")
      {
        string label = "percy";
        LogMessage(message, label);
      }
      else if (logLevel == "warn")
      {
        string label = "percy:dotnet";
        LogMessage(message, label, "93m");
      }
    }

    private static void LogMessage(String message, String label, String color = "39m")
    {
      Console.WriteLine($"[\u001b[35m{label}\u001b[{color}] {message}");
    }
  }
}
