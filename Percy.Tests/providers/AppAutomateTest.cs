using System;
using Newtonsoft.Json.Linq;
using Moq;
using Xunit;
using PercyIO.Appium;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Runtime.InteropServices.JavaScript;
using RichardSzalay.MockHttp;
using System.Net.Http;

namespace Percy.Tests
{
  public class AppAutomateTest
  {
    private readonly Mock<IPercyAppiumDriver> _androidPercyAppiumDriver = new Mock<IPercyAppiumDriver>();

    public AppAutomateTest()
    {
      _androidPercyAppiumDriver = MetadataBuilder.mockDriver("Android");
    }

    [Fact]
    public void TestSupports_WhenNotNull()
    {
      // Arrange
      String url = "http://hub-cloud.browserstack.com/wd/hub";
      _androidPercyAppiumDriver.Setup(x => x.GetHost())
        .Returns(url);
      // Act
      bool actual = AppAutomate.Supports(_androidPercyAppiumDriver.Object);
      // Assert
      Assert.True(actual);
    }

    [Fact]
    public void TestSupports_WhenNull()
    {
      // Arrange
      String url = "http://hub-cloud.abc.com/wd/hub";
      _androidPercyAppiumDriver.Setup(x => x.GetHost())
        .Returns(url);
      // Act
      bool actual = AppAutomate.Supports(_androidPercyAppiumDriver.Object);
      // Assert
      Assert.False(actual);
    }

    [Fact]
    public void TestGetDebugUrl()
    {
      // Arrange
      string json = @"{success:'true', osVersion:'11.2', buildHash:'abc', sessionHash:'def'}";
      JObject result = JObject.Parse(json);
      string expected = "https://app-automate.browserstack.com/dashboard/v2/builds/abc/sessions/def";
      // Act
      AppAutomate appAutomate = new AppAutomate(_androidPercyAppiumDriver.Object);
      string actual = appAutomate.GetDebugUrl(result);
      // Assert
      Assert.Equal(actual, expected);
    }

    [Fact]
    public void TestScreenshot()
    {
      // Arrange
      string expected = "https://percy.io/api/v1/comparisons/redirect?snapshot[name]=test%20screenshot&tag[name]=Samsung&tag[os_name]=Android&tag[os_version]=9&tag[width]=1280&tag[height]=1420&tag[orientation]=landscape";
      Environment.SetEnvironmentVariable("PERCY_DISABLE_REMOTE_UPLOADS", "true");
      var response = @"{success:'true', osVersion:'11.2', buildHash:'abc', sessionHash:'def'}";
      _androidPercyAppiumDriver.Setup(x => x.ExecuteScript(It.IsAny<string>()))
        .Returns(response);
      AppAutomate appAutomate = new AppAutomate(_androidPercyAppiumDriver.Object);
      var options = new ScreenshotOptions();
      options.DeviceName = "Samsung";
      options.StatusBarHeight = 100;
      options.NavBarHeight = 100;
      options.Orientation = "potrait";
      options.FullScreen = false;
      options.FullPage = false;
      options.ScreenLengths = 0;

      var data = JObject.FromObject(new
      {
        snapshotname = "temp",
        status = "success"
      });

      var mockHttp = new MockHttpMessageHandler();

      // Setup a respond for the user api (including a wildcard in the URL)
      mockHttp.When("http://localhost:5338/percy/comparison")
        .Respond("application/json", "{\"success\": true, \"link\": \"" + expected + "\", \"data\": \"" + data + "\"}");  

      CliWrapper.setHttpClient(new HttpClient(mockHttp));

      // Act
      var actual = appAutomate.Screenshot("temp", options);
      // Assert
      Assert.Equal(actual, null);
      CliWrapper.resetHttpClient();
    }

    [Fact]
    public void TestScreenshot_WhenPercyScreenshotBeginReturnsNull()
    {
      // Arrange
      Environment.SetEnvironmentVariable("PERCY_DISABLE_REMOTE_UPLOADS", "true");
      _androidPercyAppiumDriver.Setup(x => x.ExecuteScript(It.IsAny<string>()))
        .Throws(new Exception());
      var options = new ScreenshotOptions();
      options.DeviceName = "Samsung";
      options.StatusBarHeight = 100;
      options.NavBarHeight = 100;
      options.Orientation = "potrait";
      options.FullScreen = false;
      options.FullPage = false;
      options.ScreenLengths = 0;

      AppAutomate appAutomate = new AppAutomate(_androidPercyAppiumDriver.Object);
      // Act
      var actual = appAutomate.Screenshot("temp", options);
      // Assert
      Assert.Equal(actual, null);
      Environment.SetEnvironmentVariable("PERCY_DISABLE_REMOTE_UPLOADS", "false");
    }

    [Fact]
    public void TestExecutePercyScreenshotBegin()
    {
      // Arrange
      var arguments = new JObject();
      var response = JObject.FromObject(new
      {
        success = true,
        deviceName = "iPhone 13",
        osVersion = "15.0",
        buildHash = "dummy_build_hash",
        sessionHash = "dummy_session_hash"
      });
      string name = "First";
      var obj = JObject.FromObject(new
      {
        action = "percyScreenshot",
        arguments = new
        {
          state = "begin",
          percyBuildId = Env.GetPercyBuildID(),
          percyBuildUrl = Env.GetPercyBuildUrl(),
          name = name
        }
      });

      _androidPercyAppiumDriver.Setup(x => x.ExecuteScript("browserstack_executor:" + obj.ToString()))
        .Returns(response.ToString());
      // Act
      var appAutomate = new AppAutomate(_androidPercyAppiumDriver.Object);
      var result = appAutomate.ExecutePercyScreenshotBegin(name);
      string actual = result.GetValue("success").ToString();
      // Assert
      Assert.Equal(actual, "True");
      _androidPercyAppiumDriver.Verify(x => x.ExecuteScript("browserstack_executor:" + obj.ToString()), Times.Once);
    }

    [Fact]
    public void TestExecutePercyScreenshotBegin_WhenThrowError()
    {
      // Arrange
      var arguments = new JObject();
      var name = "First";
      var reqObject = JObject.FromObject(new
      {
        action = "percyScreenshot",
        arguments = new
        {
          state = "begin",
          percyBuildId = Env.GetPercyBuildID(),
          percyBuildUrl = Env.GetPercyBuildUrl(),
          name = name
        }
      });
      _androidPercyAppiumDriver.Setup(x => x.ExecuteScript("browserstack_executor:" + reqObject.ToString()))
        .Throws(new Exception());
      // Act
      var appAutomate = new AppAutomate(_androidPercyAppiumDriver.Object);
      // Assert
      Assert.Throws<NullReferenceException>(() => appAutomate.ExecutePercyScreenshotBegin(name).GetValue("success").ToString());
    }

    [Fact]
    public void TestExecutePercyScreenshotEnd()
    {
      // Arrange
      Environment.SetEnvironmentVariable("PERCY_LOGLEVEL", "debug");
      var response = JObject.FromObject(new
      {
        success = true,
      });
      var name = "First";
      var percyScreenshotUrl = "";
      var reqObject = JObject.FromObject(new
      {
        action = "percyScreenshot",
        arguments = new
        {
          state = "end",
          percyScreenshotUrl = percyScreenshotUrl,
          status = "success",
          statusMessage = JValue.CreateNull(),
          name = name,
          sync = false
        }
      });

      _androidPercyAppiumDriver.Setup(x => x.ExecuteScript("browserstack_executor:" + reqObject.ToString()))
        .Returns(response.ToString());
      // Act
      var appAutomate = new AppAutomate(_androidPercyAppiumDriver.Object);
      var result = appAutomate.ExecutePercyScreenshotEnd(name, percyScreenshotUrl, false, null);
      var actual = result.GetValue("success").ToString();
      // Assert
      Assert.Equal(actual, "True");
      _androidPercyAppiumDriver.Verify(x => x.ExecuteScript("browserstack_executor:" + reqObject.ToString()), Times.Once);
    }

    [Fact]
    public void TestExecutePercyScreenshotEnd_WhenError()
    {
      // Arrange
      Environment.SetEnvironmentVariable("PERCY_LOGLEVEL", "debug");
      var response = JObject.FromObject(new
      {
        success = false,
      });
      var name = "First";
      var percyScreenshotUrl = "";
      var reqObject = JObject.FromObject(new
      {
        action = "percyScreenshot",
        arguments = new
        {
          state = "end",
          percyScreenshotUrl = percyScreenshotUrl,
          status = "failure",
          statusMessage = "some error",
          name = name,
          sync = false
        }
      });
      _androidPercyAppiumDriver.Setup(x => x.ExecuteScript("browserstack_executor:" + reqObject.ToString()))
        .Returns(response.ToString());
      // Act
      var appAutomate = new AppAutomate(_androidPercyAppiumDriver.Object);
      string actual = appAutomate.ExecutePercyScreenshotEnd(name, percyScreenshotUrl, false, "some error").GetValue("success").ToString();
      // Assert
      Assert.Equal(actual, "False");
      _androidPercyAppiumDriver.Verify(x => x.ExecuteScript("browserstack_executor:" + reqObject.ToString()), Times.Once);
    }

    [Fact]
    public void TestExecutePercyScreenshotEnd_WhenException()
    {
      // Arrange
      Environment.SetEnvironmentVariable("PERCY_LOGLEVEL", "debug");
      var name = "First";
      var percyScreenshotUrl = "";
      var reqObject = JObject.FromObject(new
      {
        action = "percyScreenshot",
        arguments = new
        {
          state = "end",
          percyScreenshotUrl = percyScreenshotUrl,
          status = "failure",
          statusMessage = "some error",
          name = name
        }
      });
      _androidPercyAppiumDriver.Setup(x => x.ExecuteScript("browserstack_executor:" + reqObject.ToString()))
        .Throws(new Exception());
      // Act
      var app = new AppAutomate(_androidPercyAppiumDriver.Object);
      // Assert
      Assert.Throws<NullReferenceException>(() => app.ExecutePercyScreenshotEnd(null, null, false, null).GetValue("success").ToString());
      _androidPercyAppiumDriver.Verify(x => x.ExecuteScript("browserstack_executor:" + reqObject.ToString()), Times.Never);
    }

    [Fact]
    public void CaptureTiles_ShouldReturnListOfTiles_WhenCalled()
    {
      // Arrange
      Environment.SetEnvironmentVariable("PERCY_DISABLE_REMOTE_UPLOADS", "false");
      _androidPercyAppiumDriver.Setup(x => x.ExecuteScript(It.IsAny<string>()))
        .Returns(JsonConvert.SerializeObject(new
        {
          success = true,
          result = JsonConvert.SerializeObject(new List<object> {
              new { sha = "abcd-1234", header_height = 50, footer_height = 30 },
              new { sha = "abce-1234", header_height = 80, footer_height = 10 }
            })
        }
        ));

      var appAutomate = new AppAutomate(_androidPercyAppiumDriver.Object);
      var metadata = new AndroidMetadata(_androidPercyAppiumDriver.Object, "Samsung Galaxy s22", 100, 200, null, null);
      appAutomate.metadata = metadata;
      var options = new ScreenshotOptions();
      options.FullScreen = false;
      options.FullPage = true;
      options.ScreenLengths = 2;
      // Act
      var result = appAutomate.CaptureTiles(options);

      // Assert
      Assert.IsType<System.Collections.Generic.List<Tile>>(result);
      Assert.Equal(2, result.Count);
      Assert.Equal("abcd", result[0].Sha);
      Assert.Equal(100, result[0].StatusBarHeight);
      Assert.Equal(200, result[0].NavBarHeight);
      Assert.Equal(50, result[0].HeaderHeight);
      Assert.Equal(30, result[0].FooterHeight);
    }

    [Fact]
    public void TestExecutePercyScreenshot()
    {
      var response = JsonConvert.SerializeObject(new
      {
        success = true,
        result = JsonConvert.SerializeObject(new List<object> {
            new { sha = "abcd-1234", header_height = 50, footer_height = 30 }
          })
      }
      );
      _androidPercyAppiumDriver.Setup(x => x.ExecuteScript(It.IsAny<string>())).Returns(response);

      var options = new ScreenshotOptions();
      options.ScreenLengths = 2;
      options.ScrollableXpath = "xapth/dummy/scrollable";
      // Act
      var appAutomate = new AppAutomate(_androidPercyAppiumDriver.Object);
      var metadata = new AndroidMetadata(_androidPercyAppiumDriver.Object, "Samsung Galaxy s22", 100, 200, null, null);
      appAutomate.metadata = metadata;
      var actual = appAutomate.ExecutePercyScreenshot(options);
      // Assert
      _androidPercyAppiumDriver.Verify(x => x.ExecuteScript(It.IsAny<string>()), Times.Once);
      Assert.Contains("abcd-1234", actual);
    }

    [Fact]
    public void TestDeviceName_WhenValueIsNull()
    {
      // Arrange
      var json = @"{deviceName:'Samsung Galaxy S22'}";
      var result = JObject.Parse(json);
      var expected = "Samsung Galaxy S22";
      // Act
      var appAutomate = new AppAutomate(_androidPercyAppiumDriver.Object);
      var actual = appAutomate.DeviceName(null, result);
      // Assert
      Assert.Equal(actual, expected);
    }

    [Fact]
    public void TestDeviceName_WhenProvidedInParams()
    {
      // Arrange
      var json = @"{deviceName:'Samsung Galaxy S22'}";
      var result = JObject.Parse(json);
      var expected = "Samsung Galaxy S21";
      // Act
      var appAutomate = new AppAutomate(_androidPercyAppiumDriver.Object);
      var actual = appAutomate.DeviceName(expected, result);
      // Assert
      Assert.Equal(actual, expected);
    }

    [Fact]
    public void TestVerifyCorrectAppiumVersion_WhenJSONFalse()
    {
      var expected = false;
      _androidPercyAppiumDriver.Setup(x => x.GetCapabilities().getValue<String>("browserstack.appium_version")).Returns("1.16.0");
      // Act
      var appAutomate = new AppAutomate(_androidPercyAppiumDriver.Object);
      var actual = appAutomate.VerifyCorrectAppiumVersion();
      // Assert
      Assert.Equal(actual, expected);
    }

    [Fact]
    public void TestVerifyCorrectAppiumVersion_WhenJSONTrue()
    {
      var expected = true;
      _androidPercyAppiumDriver.Setup(x => x.GetCapabilities().getValue<String>("browserstack.appium_version")).Returns("1.20.0");
      // Act
      var appAutomate = new AppAutomate(_androidPercyAppiumDriver.Object);
      var actual = appAutomate.VerifyCorrectAppiumVersion();
      // Assert
      Assert.Equal(actual, expected);
    }

    [Fact]
    public void TestVerifyCorrectAppiumVersion_WhenW3CFalse()
    {
      var expected = false;
      _androidPercyAppiumDriver.Setup(x => x.GetCapabilities().getValue<Dictionary<string, object>>("bstack:options")).Returns(
        new Dictionary<string, object> {
          {"appiumVersion", "1.16.0"},
        }
      );
      // Act
      var appAutomate = new AppAutomate(_androidPercyAppiumDriver.Object);
      var actual = appAutomate.VerifyCorrectAppiumVersion();
      // Assert
      Assert.Equal(actual, expected);
    }

    [Fact]
    public void TestVerifyCorrectAppiumVersion_WhenW3CTrue()
    {
      var expected = true;
      _androidPercyAppiumDriver.Setup(x => x.GetCapabilities().getValue<Dictionary<string, object>>("bstack:options")).Returns(
        new Dictionary<string, object> {
          {"appiumVersion", "1.20.0"},
        }
      );
      // Act
      var appAutomate = new AppAutomate(_androidPercyAppiumDriver.Object);
      var actual = appAutomate.VerifyCorrectAppiumVersion();
      // Assert
      Assert.Equal(actual, expected);
    }
  }
}
