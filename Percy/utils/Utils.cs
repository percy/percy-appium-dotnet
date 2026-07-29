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

    // Appium/Selenium exception text embeds the command-executor URI, commonly supplied as
    // https://user:accesskey@hub-cloud.browserstack.com/wd/hub. Applied inside LogMessage so
    // every call site is covered.
    // Keyed on the scheme because GenericProvider logs locators ("xpath://a[@id='x']", "id:...")
    // that carry `://` and `@` but never an http/ws scheme. Matching on userinfo content instead
    // failed open twice — a {1,512} bound (a ~680-char JWT matched nothing) and an RFC 3986
    // allow-list (one unlisted char, e.g. the `[` of a generated password, leaked the whole URL);
    // a non-match prints the credential in full. Excluding `/` keeps a match out of the path.
    private static readonly Regex UrlUserInfo =
      new Regex(@"\b(https?|wss?)://[^\s@/]+@", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    private static readonly Regex CredentialQuery =
      new Regex(@"([?&](?:access[_-]?key|auth[_-]?token|token|password|secret)=)[^&\s""']+",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public static String RedactCredentials(String message)
    {
      if (String.IsNullOrEmpty(message)) return message;
      message = UrlUserInfo.Replace(message, "$1://***@");
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
