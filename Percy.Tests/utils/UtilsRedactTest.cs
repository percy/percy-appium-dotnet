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
      Assert.Contains("//***:***@hub-cloud.browserstack.com/wd/hub", actual);
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
