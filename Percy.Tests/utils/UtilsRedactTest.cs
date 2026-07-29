using System;
using System.IO;
using Xunit;
using PercyIO.Appium;

namespace Percy.Tests
{
  public class UtilsRedactTest
  {
    [Fact]
    public void StripsUserInfoFromHubUrls()
    {
      // Appium exception text embeds the command-executor URI, and App Automate users commonly
      // pass credentials inline. That must not reach an always-on log line.
      var msg = "The HTTP request to https://myuser:s3cr3tkey@hub-cloud.browserstack.com/wd/hub timed out";
      var actual = Utils.RedactCredentials(msg);
      Assert.DoesNotContain("s3cr3tkey", actual);
      Assert.DoesNotContain("myuser", actual);
      Assert.Contains("://***@hub-cloud.browserstack.com/wd/hub", actual);
    }

    [Fact]
    public void StripsUserInfoWithoutAColon()
    {
      var actual = Utils.RedactCredentials("connect to https://sometoken@hub.browserstack.com/wd/hub failed");
      Assert.DoesNotContain("sometoken", actual);
      Assert.Contains("://***@hub.browserstack.com", actual);
    }

    [Fact]
    public void StripsCredentialQueryParameters()
    {
      var actual = Utils.RedactCredentials("GET /session?accessKey=abc123&other=keep");
      Assert.DoesNotContain("abc123", actual);
      Assert.Contains("accessKey=***", actual);
      Assert.Contains("other=keep", actual);
    }

    [Fact]
    public void RedactsAtTheLogChokePoint()
    {
      // Applied inside LogMessage, so every call site is covered without per-site wrapping
      var stdout = new StringWriter();
      var original = Console.Out;
      Console.SetOut(stdout);
      try
      {
        Utils.Log("failed against https://me:supersecret@hub-cloud.browserstack.com/wd/hub");
        Assert.DoesNotContain("supersecret", stdout.ToString());
      }
      finally
      {
        Console.SetOut(original);
      }
    }

    [Fact]
    public void LeavesXpathAndOrdinaryTextUntouched()
    {
      // Redaction runs on every log line, so over-redaction is as damaging as under-redaction.
      // Element-not-found is the most common Appium failure text, and an unanchored pattern
      // mangled it into "//***@text='OK']".
      var xpath = "Appium Element with xpath://android.widget.Button[@text='OK'] not found.";
      Assert.Equal(xpath, Utils.RedactCredentials(xpath));
      Assert.Equal("//XCUIElementTypeButton[@name=\"Login\"]",
        Utils.RedactCredentials("//XCUIElementTypeButton[@name=\"Login\"]"));
      Assert.Equal("//example.com/a@b", Utils.RedactCredentials("//example.com/a@b"));
      Assert.Equal("?csrf_token_name=safe", Utils.RedactCredentials("?csrf_token_name=safe"));
      Assert.Equal("?tokens=3", Utils.RedactCredentials("?tokens=3"));
      // Sub-delims are in the userinfo class, so confirm an XPath carrying them is still safe:
      // the predicate bracket has to be crossed to reach the `@`, and `[` is excluded.
      Assert.Equal("xpath://a[@id='x,y' and @n=\"1\"]",
        Utils.RedactCredentials("xpath://a[@id='x,y' and @n=\"1\"]"));
      Assert.Equal("//android.widget.EditText/@text",
        Utils.RedactCredentials("//android.widget.EditText/@text"));
      Assert.Equal("xpath://*[@resource-id='x']", Utils.RedactCredentials("xpath://*[@resource-id='x']"));
      Assert.Equal("id:com.example:id/btn", Utils.RedactCredentials("id:com.example:id/btn"));
    }

    [Fact]
    public void RedactsLongUserinfoRatherThanFailingOpen()
    {
      // A length cap on the userinfo quantifier fails open: past the cap nothing matches and the
      // whole URL prints. A JWT in the basic-auth password position is the realistic case.
      var jwt = new string('A', 680);
      var redacted = Utils.RedactCredentials($"https://svc:{jwt}@hub.example.com/wd/hub");
      Assert.Equal("https://***@hub.example.com/wd/hub", redacted);
      Assert.DoesNotContain(jwt, redacted);
    }

    [Fact]
    public void RedactsCredentialsContainingSubDelimiters()
    {
      // A userinfo character outside the class makes the match fail outright rather than
      // degrade, leaking the whole URL — so a self-hosted grid on basic auth with a
      // symbol-bearing password is the case that must not regress.
      var url = "https://my.user!:pa$$w&rd@hub.example.com/wd/hub";
      var redacted = Utils.RedactCredentials(url);
      Assert.Equal("https://***@hub.example.com/wd/hub", redacted);
      Assert.DoesNotContain("pa$$w&rd", redacted);
      Assert.DoesNotContain("my.user!", redacted);
    }

    [Fact]
    public void RedactsPasswordsOutsideTheUrlCharacterSet()
    {
      // An allow-list fails open: the whole run must match, so one unlisted character prints
      // the credential in full. `Uri` preserves brackets verbatim, and passwords often carry them.
      var bracketed = Utils.RedactCredentials("https://user:p[a]ss@grid.corp.local:4444/wd/hub");
      Assert.Equal("https://***@grid.corp.local:4444/wd/hub", bracketed);
      Assert.DoesNotContain("p[a]ss", bracketed);

      var hashed = Utils.RedactCredentials("http://user:p#ssword@hub.example.com/wd/hub");
      Assert.Equal("http://***@hub.example.com/wd/hub", hashed);
      Assert.DoesNotContain("p#ssword", hashed);
    }

    [Fact]
    public void RedactsRegardlessOfSchemeCasingAndCount()
    {
      // Casing must not decide redaction, and one URL must not consume the rest of the line.
      Assert.Equal("HTTPS://***@hub.example.com/wd/hub",
        Utils.RedactCredentials("HTTPS://user:secret@hub.example.com/wd/hub"));

      var two = Utils.RedactCredentials(
        "tried https://a:one@h1.example.com/wd/hub then wss://b:two@h2.example.com/ws");
      Assert.DoesNotContain("one", two);
      Assert.DoesNotContain("two", two);
      Assert.Equal("tried https://***@h1.example.com/wd/hub then wss://***@h2.example.com/ws", two);
    }

    [Fact]
    public void StripsTheOtherCredentialQueryKeys()
    {
      Assert.Contains("access-key=***", Utils.RedactCredentials("?access-key=abc123"));
      Assert.Contains("auth_token=***", Utils.RedactCredentials("?auth_token=abc123"));
      Assert.Contains("password=***", Utils.RedactCredentials("?password=abc123"));
      Assert.Contains("secret=***", Utils.RedactCredentials("?secret=abc123"));
      Assert.DoesNotContain("abc123", Utils.RedactCredentials("?secret=abc123"));
    }

    [Fact]
    public void LeavesOrdinaryMessagesUntouched()
    {
      var msg = "The given key 'appiumVersion' was not present in the dictionary.";
      Assert.Equal(msg, Utils.RedactCredentials(msg));
      Assert.Null(Utils.RedactCredentials(null));
      Assert.Equal("", Utils.RedactCredentials(""));
    }
  }
}
