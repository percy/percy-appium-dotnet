using System;
using System.Text.RegularExpressions;

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

    // Appium/Selenium exception text routinely embeds the command-executor URI, and App Automate
    // users commonly supply that as https://user:accesskey@hub-cloud.browserstack.com/wd/hub.
    // Applied inside LogMessage so every call site — present and future — is covered.
    // Both halves of the userinfo pattern are load-bearing. Without the `://` anchor it matches
    // a bare XPath, and `://` alone is not enough either — GenericProvider logs
    // "xpath:" + "//android.widget.Button[@text='OK']", which contains `://` too. Restricting
    // userinfo to characters legal in a URL means the bracket in an XPath predicate ends the
    // match before any `@`. Element-not-found is the most common Appium failure text that
    // passes through here, so over-redaction costs as much as under-redaction.
    private static readonly Regex UrlUserInfo =
      new Regex(@"://[A-Za-z0-9._~%+\-:]+@", RegexOptions.Compiled);
    private static readonly Regex CredentialQuery =
      new Regex(@"([?&](?:access[_-]?key|auth[_-]?token|token|password|secret)=)[^&\s""']+",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public static String RedactCredentials(String message)
    {
      if (String.IsNullOrEmpty(message)) return message;
      message = UrlUserInfo.Replace(message, "://***@");
      return CredentialQuery.Replace(message, "$1***");
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
      Console.WriteLine($"[\u001b[35m{label}\u001b[{color}] {RedactCredentials(message)}");
    }
  }
}
