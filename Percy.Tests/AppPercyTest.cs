using System;
using Xunit;
using PercyIO.Appium;
using RichardSzalay.MockHttp;
using System.Net.Http;
using Newtonsoft.Json;
using System.IO;

namespace Percy.Tests
{
  public class AppPercyTest
  {

    private static String LogMessage(String label, String message, String color = "39m")
    {
      return $"[\u001b[35m{label}\u001b[{color}] {message}";
    }

    [Fact]
    public void TestName_WithAndroid_V4()
    {
      // Arrange
      AppPercy.cache.Clear();
      TestHelper.UnsetEnvVariables();
      var mockDriver = new MockDriverObject();
      mockDriver.SessionId = "session-1";
      mockDriver.SetCapability(MetadataBuilder.CapabilityBuilder("Android"));
      mockDriver.setCommandExecutor("https://browserstack.com/wd/hub");
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

      StringWriter stringWriter = new StringWriter();
      Console.SetOut(stringWriter);

      // Act
      var name = "dummyName";
      AppPercy appPercy = new AppPercy(mockDriver);
      appPercy.Screenshot("abc", null);
      var expectedOutput = LogMessage("percy:dotnet", "Driver object is not the type of AndroidDriver or IOSDriver. The percy command may break.", "93m") + Environment.NewLine;
      var actualOutput = stringWriter.ToString();
      var errorOutput = LogMessage("percy", $"Error taking screenshot {name}") +  Environment.NewLine;
      // Assert
      Assert.Equal(expectedOutput, actualOutput);
      Assert.NotEqual(errorOutput, actualOutput);
    }

    [Fact]
    public void TestName_WithIOS_V4()
    {
      // Arrange
      AppPercy.cache.Clear();
      TestHelper.UnsetEnvVariables();
      var mockDriver = new MockDriverObject();
      mockDriver.SessionId = "session-1";
      mockDriver.SetCapability(MetadataBuilder.CapabilityBuilder("iOS"));
      mockDriver.setCommandExecutor("https://browserstack.com/wd/hub");
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

      StringWriter stringWriter = new StringWriter();
      Console.SetOut(stringWriter);

      // Act
      AppPercy appPercy = new AppPercy(mockDriver);
      var name = "dummyName";
      appPercy.Screenshot(name, null);
      var expectedOutput = LogMessage("percy:dotnet", "Driver object is not the type of AndroidDriver or IOSDriver. The percy command may break.", "93m") + Environment.NewLine;
      var actualOutput = stringWriter.ToString();
      var errorOutput = LogMessage("percy", $"Error taking screenshot {name}") +  Environment.NewLine;
      // Assert
      Assert.Equal(expectedOutput, actualOutput);
      Assert.NotEqual(errorOutput, actualOutput);
    }

    [Fact]
    public void TestName_WithAndroid_V5()
    {
      // Arrange
      AppPercy.cache.Clear();
      TestHelper.UnsetEnvVariables();
      var mockDriver = new MockDriverObject();
      mockDriver.SessionId = "session-1";
      mockDriver.SetCapability(MetadataBuilder.CapabilityBuilder("Android"));
      // Setting v5
      mockDriver.setIsV5(true);
      mockDriver.setCommandExecutor("https://browserstack.com/wd/hub");
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

      StringWriter stringWriter = new StringWriter();
      Console.SetOut(stringWriter);

      // Act
      var name = "dummyName";
      AppPercy appPercy = new AppPercy(mockDriver);
      appPercy.Screenshot("abc", null);
      var expectedOutput = LogMessage("percy:dotnet", "Driver object is not the type of AndroidDriver or IOSDriver. The percy command may break.", "93m") + Environment.NewLine;
      var actualOutput = stringWriter.ToString();
      var errorOutput = LogMessage("percy", $"Error taking screenshot {name}") +  Environment.NewLine;
      // Assert
      Assert.Equal(expectedOutput, actualOutput);
      Assert.NotEqual(errorOutput, actualOutput);
    }

    [Fact]
    public void TestName_WithIOS_V5()
    {
      // Arrange
      AppPercy.cache.Clear();
      TestHelper.UnsetEnvVariables();
      var mockDriver = new MockDriverObject();
      mockDriver.SessionId = "session-1";
      mockDriver.SetCapability(MetadataBuilder.CapabilityBuilder("iOS"));
      // Setting v5
      mockDriver.setIsV5(true);
      mockDriver.setCommandExecutor("https://browserstack.com/wd/hub");
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

      StringWriter stringWriter = new StringWriter();
      Console.SetOut(stringWriter);

      // Act
      AppPercy appPercy = new AppPercy(mockDriver);
      var name = "dummyName";
      appPercy.Screenshot(name, null);
      var expectedOutput = LogMessage("percy:dotnet", "Driver object is not the type of AndroidDriver or IOSDriver. The percy command may break.", "93m") + Environment.NewLine;
      var actualOutput = stringWriter.ToString();
      var errorOutput = LogMessage("percy", $"Error taking screenshot {name}") +  Environment.NewLine;
      // Assert
      Assert.Equal(expectedOutput, actualOutput);
      Assert.NotEqual(errorOutput, actualOutput);
    }
  }
}
