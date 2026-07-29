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
    //
    // The hard part is not matching credentials, it is not mangling locators: GenericProvider
    // logs "xpath:" + "//android.widget.Button[@text='OK']", which contains `://` and an `@`,
    // and element-not-found is the most common Appium failure text to pass through here. So
    // over-redaction costs as much as under-redaction.
    //
    // Discriminate on the scheme, not on what a password may contain. Two earlier attempts both
    // failed open — the only failure mode that matters, since a non-match prints the credential
    // in full rather than degrading:
    //   - a `{1,512}` bound on the userinfo run: a JWT in the password position reaches ~680
    //     characters and matched nothing.
    //   - an allow-list of the RFC 3986 userinfo set: the whole run has to match, so one
    //     character outside the set leaked everything. `[` is the reachable one — .NET's `Uri`
    //     rejects `#` and `\` and percent-encodes space and quotes, but preserves brackets
    //     verbatim, and brackets are common in generated passwords.
    // Inverting the class does not fix that either, because `[` and `/` must stay excluded for
    // the locator property to hold. Requiring an http/ws scheme is what makes the exclusion
    // unnecessary: a locator is logged as `xpath://…` or `id:…` and never carries one.
    // Excluding `/` from the userinfo run is RFC-correct — a `/` ends the authority — so a match
    // cannot bridge into a path. A literal space in userinfo is the one case skipped, and it
    // cannot appear in a URL; `Uri` percent-encodes it to `%20`, which does match.
    // No bound is needed for ReDoS: single quantifier, no nesting, no alternation inside the run.
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
