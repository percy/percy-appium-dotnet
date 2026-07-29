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
