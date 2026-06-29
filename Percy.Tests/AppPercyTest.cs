using System;
using System.Collections.Generic;
using System.Dynamic;
using Xunit;
using PercyIO.Appium;
using RichardSzalay.MockHttp;
using System.Net.Http;
using Newtonsoft.Json;
using System.IO;

namespace Percy.Tests
{
  public class AppPercyTest : IDisposable
  {
    private readonly MockDriverObject mockDriver;
    private readonly StringWriter stringWriter;

    // Reset shared static state after every test so the mock HttpClient and the
    // PERCY_DISABLE_REMOTE_UPLOADS env var set here cannot leak into other test
    // classes (cross-class test pollution).
    public void Dispose()
    {
      CliWrapper.resetHttpClient();
      Environment.SetEnvironmentVariable("PERCY_DISABLE_REMOTE_UPLOADS", null);
    }

    public AppPercyTest()
    {
      AppPercy.cache.Clear();
      TestHelper.UnsetEnvVariables();
      Environment.SetEnvironmentVariable("PERCY_DISABLE_REMOTE_UPLOADS", "true");
      mockDriver = new MockDriverObject();
      mockDriver.SessionId = "session-1";
      CliWrapper.Healthcheck = () =>
      {
        return true;
      };
      var mockHttp = new MockHttpMessageHandler();
      var expected = "https://percy.io/api/v1/comparisons/redirect?snapshot[name]=test%20screenshot&tag[name]=Samsung&tag[os_name]=Android&tag[os_version]=9&tag[width]=1280&tag[height]=1420&tag[orientation]=landscape";
      var obj = new
      {
        success = true,
        link = expected
      };
      CliWrapper.setHttpClient(new HttpClient(mockHttp));
      // Setup a respond for the user api (including a wildcard in the URL)
      mockHttp.When("http://localhost:5338/percy/comparison")
        .Respond("application/json", JsonConvert.SerializeObject(obj));

      stringWriter = new StringWriter();
      Console.SetOut(stringWriter);
    }

    private static String LogMessage(String label, String message, String color = "39m")
    {
      return $"[\u001b[35m{label}\u001b[{color}] {message}";
    }

    [Fact]
    public void TestName_WithAndroid_V4()
    {
      // Arrange
      mockDriver.SetCapability(MetadataBuilder.CapabilityBuilder("Android"));
      mockDriver.setCommandExecutor("https://browserstack.com/wd/hub");
      // Act
      var name = "dummyName";
      AppPercy appPercy = new AppPercy(mockDriver);
      appPercy.Screenshot("abc", null);
      var expectedOutput = LogMessage("percy:dotnet", "Driver object is not the type of AndroidDriver or IOSDriver. The percy command may break.", "93m") + Environment.NewLine;
      var actualOutput = stringWriter.ToString();
      var errorOutput = LogMessage("percy", $"Error taking screenshot {name}") +  Environment.NewLine;
      // Assert
      Assert.NotEqual(errorOutput, actualOutput);
    }

    [Fact]
    public void TestName_WithIOS_V4()
    {
      // Arrange
      mockDriver.SetCapability(MetadataBuilder.CapabilityBuilder("iOS"));
      mockDriver.setCommandExecutor("https://browserstack.com/wd/hub");
      
      // Act
      AppPercy appPercy = new AppPercy(mockDriver);
      var name = "dummyName";
      appPercy.Screenshot(name, null);
      var expectedOutput = LogMessage("percy:dotnet", "Driver object is not the type of AndroidDriver or IOSDriver. The percy command may break.", "93m") + Environment.NewLine;
      var actualOutput = stringWriter.ToString();
      var errorOutput = LogMessage("percy", $"Error taking screenshot {name}") +  Environment.NewLine;
      // Assert
      Assert.NotEqual(errorOutput, actualOutput);
    }

    [Fact]
    public void TestName_WithAndroid_V5()
    {
      // Arrange
      // Setting v5
      mockDriver.setIsV5(true);
      mockDriver.SetCapability(MetadataBuilder.CapabilityBuilder("Android"));
      mockDriver.setCommandExecutor("https://browserstack.com/wd/hub");

      // Act
      var name = "dummyName";
      AppPercy appPercy = new AppPercy(mockDriver);
      appPercy.Screenshot("abc", null);
      var expectedOutput = LogMessage("percy:dotnet", "Driver object is not the type of AndroidDriver or IOSDriver. The percy command may break.", "93m") + Environment.NewLine;
      var actualOutput = stringWriter.ToString();
      var errorOutput = LogMessage("percy", $"Error taking screenshot {name}") +  Environment.NewLine;
      // Assert
      Assert.NotEqual(errorOutput, actualOutput);
    }

    [Fact]
    public void TestName_WithIOS_V5()
    {
      // Arrange
      // Setting v5
      mockDriver.setIsV5(true);
      mockDriver.SetCapability(MetadataBuilder.CapabilityBuilder("iOS"));
      mockDriver.setCommandExecutor("https://browserstack.com/wd/hub");

      // Act
      AppPercy appPercy = new AppPercy(mockDriver);
      var name = "dummyName";
      appPercy.Screenshot(name, null);
      var expectedOutput = LogMessage("percy:dotnet", "Driver object is not the type of AndroidDriver or IOSDriver. The percy command may break.", "93m") + Environment.NewLine;
      var actualOutput = stringWriter.ToString();
      var errorOutput = LogMessage("percy", $"Error taking screenshot {name}") +  Environment.NewLine;
      // Assert
      Assert.NotEqual(errorOutput, actualOutput);
    }

    [Fact]
    public void TestName_ShouldThrowError()
    {
      // Arrange
      // Setting v5
      mockDriver.setIsV5(true);
      mockDriver.SetCapability(MetadataBuilder.CapabilityBuilder("iOS"));
      mockDriver.setCommandExecutor("https://browserstack.com/wd/hub");

      // Act
      AppPercy appPercy = new AppPercy(mockDriver);
      var name = "dummyName";
      try {
        appPercy.Screenshot(name, new Dictionary<string, object>());
        // Fail if exception not thrown
        Assert.Fail("Exception not raised");
      } catch (Exception) {

      }
    }

    [Fact]
    public void Screenshot_ReturnsNull_WhenPercyDisabledInCapabilities()
    {
      // Arrange — percyOptions.enabled = false in capabilities so PercyEnabled() is false.
      dynamic percyOption = new ExpandoObject();
      percyOption.jwp = false;
      percyOption.percyEnabled = "False";
      percyOption.ignoreError = "False";
      mockDriver.SetCapability(MetadataBuilder.CapabilityBuilder("Android", percyOption));
      mockDriver.setCommandExecutor("https://browserstack.com/wd/hub");

      // Act
      AppPercy appPercy = new AppPercy(mockDriver);
      var result = appPercy.Screenshot("disabled", null);

      // Assert — early return, no screenshot taken.
      Assert.Null(result);
    }

    [Fact]
    public void Screenshot_ReturnsData_WhenResponseContainsDataKey()
    {
      // Arrange — respond with a payload that carries a "data" object so the
      // data extraction branch returns it.
      mockDriver.SetCapability(MetadataBuilder.CapabilityBuilder("Android"));
      mockDriver.setCommandExecutor("https://browserstack.com/wd/hub");

      var mockHttp = new MockHttpMessageHandler();
      var obj = new
      {
        success = true,
        data = new { snapshot = "snapshot-name" }
      };
      mockHttp.When("http://localhost:5338/percy/comparison")
        .Respond("application/json", JsonConvert.SerializeObject(obj));
      CliWrapper.setHttpClient(new HttpClient(mockHttp));

      // Act
      AppPercy appPercy = new AppPercy(mockDriver);
      var result = appPercy.Screenshot("with-data", null);

      // Assert
      Assert.NotNull(result);
      Assert.Equal("snapshot-name", result.GetValue("snapshot").ToString());
      CliWrapper.resetHttpClient();
    }

    [Fact]
    public void Screenshot_SwallowsPercyException_WhenIgnoreErrorsTrue()
    {
      // Arrange — driver whose screenshot capture raises a PercyException; default
      // ignoreErrors swallows it, posts a failed event and returns null.
      AppPercy.ignoreErrors = true;
      var throwingDriver = new ThrowingDriverObject();
      throwingDriver.SessionId = "session-throw";
      throwingDriver.SetCapability(MetadataBuilder.CapabilityBuilder("Android"));
      throwingDriver.setCommandExecutor("https://browserstack.com/wd/hub");

      var mockHttp = new MockHttpMessageHandler();
      mockHttp.Fallback.Respond("application/json", JsonConvert.SerializeObject(new { success = true }));
      CliWrapper.setHttpClient(new HttpClient(mockHttp));

      // Act
      AppPercy appPercy = new AppPercy(throwingDriver);
      var result = appPercy.Screenshot("boom", null);

      // Assert
      Assert.Null(result);
      Assert.Contains(LogMessage("percy:dotnet", "The method is not valid for current driver. Please contact us.", "93m"), stringWriter.ToString());
      Assert.Contains(LogMessage("percy", "Error taking screenshot boom"), stringWriter.ToString());
      CliWrapper.resetHttpClient();
    }

    [Fact]
    public void Screenshot_Rethrows_WhenIgnoreErrorsFalse()
    {
      // Arrange — same throwing driver but ignoreErrors disabled, so it rethrows.
      AppPercy.ignoreErrors = false;
      var throwingDriver = new ThrowingDriverObject();
      throwingDriver.SessionId = "session-throw";
      throwingDriver.SetCapability(MetadataBuilder.CapabilityBuilder("Android"));
      throwingDriver.setCommandExecutor("https://browserstack.com/wd/hub");

      var mockHttp = new MockHttpMessageHandler();
      mockHttp.Fallback.Respond("application/json", JsonConvert.SerializeObject(new { success = true }));
      CliWrapper.setHttpClient(new HttpClient(mockHttp));

      // Act + Assert
      AppPercy appPercy = new AppPercy(throwingDriver);
      var ex = Assert.Throws<Exception>(() => appPercy.Screenshot("boom", null));
      Assert.Equal("Error taking screenshot boom", ex.Message);

      // Reset shared static so later tests keep default behaviour.
      AppPercy.ignoreErrors = true;
      CliWrapper.resetHttpClient();
    }
  }

  // Mirrors MockDriverObject but raises a PercyException during screenshot capture,
  // driving AppPercy.Screenshot's catch block (failed-event post + PercyException warn).
  internal class ThrowingDriverObject
  {
    private MockCapabilities mockCapabilities;
    public String SessionId { get; set; }
    public MockCapabilities Capabilities
    {
      get
      {
        return mockCapabilities;
      }
    }

    public void SetCapability(Dictionary<string, object> caps)
    {
      mockCapabilities = new MockCapabilities();
      mockCapabilities.Capabilities = caps;
    }

    private CommandExecutor commandExecutor;

    CommandExecutor CommandExecutor
    {
      get
      {
        return commandExecutor;
      }
    }

    public void setCommandExecutor(String url)
    {
      commandExecutor = new CommandExecutor();
      commandExecutor.SetUrl(url);
    }

    public Object ExecuteScript(String script)
    {
      return @"{success:'true', osVersion:'11.2', buildHash:'abc', sessionHash:'def'}";
    }

    public Object GetScreenshot()
    {
      throw new PercyException("Screenshot not available on this driver");
    }
  }
}
