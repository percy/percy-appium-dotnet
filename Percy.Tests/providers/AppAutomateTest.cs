using System;
using OpenQA.Selenium;
using Newtonsoft.Json.Linq;
using Moq;
using Xunit;
using PercyIO.Appium;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Percy.Tests
{
  public class AppAutomateTest
  {
    private AndroidMetadata androidMetadata;
    private readonly Mock<IPercyAppiumDriver> _androidPercyAppiumDriver = new Mock<IPercyAppiumDriver>();
    private readonly Mock<ICapabilities> capabilities = new Mock<ICapabilities>();

    [Fact]
    public void TestSupports_WhenNotNull()
    {
      // Given
      String url = "http://hub-cloud.browserstack.com/wd/hub";
      _androidPercyAppiumDriver.Setup(x => x.GetHost())
        .Returns(url);
      // When
      bool actual = AppAutomate.Supports(_androidPercyAppiumDriver.Object);
      // Then
      Assert.True(actual);
    }

    [Fact]
    public void TestSupports_WhenNull()
    {
      // Given
      String url = "http://hub-cloud.abc.com/wd/hub";
      _androidPercyAppiumDriver.Setup(x => x.GetHost())
        .Returns(url);
      _androidPercyAppiumDriver.Setup(x => x.GetCapabilities())
        .Returns(capabilities.Object);
      // When
      bool actual = AppAutomate.Supports(_androidPercyAppiumDriver.Object);
      // Then
      Assert.False(actual);
    }

    [Fact]
    public void TestGetDebugUrl()
    {
      // Given
      string json = @"{success:'true', osVersion:'11.2', buildHash:'abc', sessionHash:'def'}";
      JObject result = JObject.Parse(json);
      string expected = "https://app-automate.browserstack.com/dashboard/v2/builds/abc/sessions/def";
      // When
      AppAutomate appAutomate = new AppAutomate(_androidPercyAppiumDriver.Object);
      string actual = appAutomate.GetDebugUrl(result);
      // Then
      Assert.Equal(actual, expected);
    }

    [Fact]
    public void TestScreenshot()
    {
      // Given
      var response = @"{success:'true', osVersion:'11.2', buildHash:'abc', sessionHash:'def'}";
      _androidPercyAppiumDriver.Setup(x => x.ExecuteScript(It.IsAny<string>()))
        .Returns(response);
      _androidPercyAppiumDriver.Setup(x => x.GetType()).Returns("Android");
      AppPercy.cache.Clear();
      capabilities.Setup(x => x.GetCapability("platformName"))
         .Returns("Android");
      capabilities.Setup(x => x.GetCapability("platformVersion"))
        .Returns("9");
      capabilities.Setup(x => x.GetCapability("deviceScreenSize"))
        .Returns("1280x1420");
      capabilities.Setup(x => x.GetCapability("orientation"))
        .Returns("landscape");
      _androidPercyAppiumDriver.Setup(x => x.GetCapabilities())
        .Returns(capabilities.Object);
      var screenshot = new Screenshot("c2hvcnRlc3Q=");
      _androidPercyAppiumDriver.Setup(x => x.GetScreenshot())
        .Returns(screenshot);
      AppAutomate appAutomate = new AppAutomate(_androidPercyAppiumDriver.Object);
      var options = new ScreenshotOptions();
      options.DeviceName = "Samsung";
      options.StatusBarHeight = 100;
      options.NavBarHeight = 100;
      options.Orientation = "potrait";
      options.FullScreen = false;
      options.FullPage = false;
      options.ScreenLengths = 0;
      // When
      string actual = appAutomate.Screenshot("temp", options);
      // Then
      Assert.Equal(actual, "");
    }

    [Fact]
    public void TestScreenshot_WhenPercyScreenshotBeginReturnsNull()
    {
      // Given
      _androidPercyAppiumDriver.Setup(x => x.ExecuteScript(It.IsAny<string>()))
        .Throws(new Exception());
      _androidPercyAppiumDriver.Setup(x => x.GetType()).Returns("Android");
      AppPercy.cache.Clear();
      capabilities.Setup(x => x.GetCapability("platformName"))
         .Returns("Android");
      capabilities.Setup(x => x.GetCapability("platformVersion"))
        .Returns("9");
      capabilities.Setup(x => x.GetCapability("deviceScreenSize"))
        .Returns("1280x1420");
      capabilities.Setup(x => x.GetCapability("orientation"))
        .Returns("landscape");
      _androidPercyAppiumDriver.Setup(x => x.GetCapabilities())
        .Returns(capabilities.Object);
      var screenshot = new Screenshot("c2hvcnRlc3Q=");
      _androidPercyAppiumDriver.Setup(x => x.GetScreenshot())
        .Returns(screenshot);
      var options = new ScreenshotOptions();
      options.DeviceName = "Samsung";
      options.StatusBarHeight = 100;
      options.NavBarHeight = 100;
      options.Orientation = "potrait";
      options.FullScreen = false;
      options.FullPage = false;
      options.ScreenLengths = 0;

      AppAutomate appAutomate = new AppAutomate(_androidPercyAppiumDriver.Object);
      // When
      string actual = appAutomate.Screenshot("temp", options);
      // Then
      Assert.Equal(actual, "");
    }

    [Fact]
    public void TestExecutePercyScreenshotBegin()
    {
      // Given
      var arguments = new JObject();
      var response = JObject.FromObject(new
      {
        success = true,
        deviceName = "iPhone 13",
        osVersion = "15.0",
        buildHash = "alfkjsdkfn",
        sessionHash = "lskdfksdjfb"
      });
      string name = "First";
      var obj = JObject.FromObject(new
      {
        action = "percyScreenshot",
        arguments = new
        {
          state = "begin",
          percyBuildId = Environment.GetEnvironmentVariable("PERCY_BUILD_ID"),
          percyBuildUrl = Environment.GetEnvironmentVariable("PERCY_BUILD_URL"),
          name = name
        }
      });

      _androidPercyAppiumDriver.Setup(x => x.ExecuteScript("browserstack_executor:" + obj.ToString()))
        .Returns(response.ToString());
      // When
      var appAutomate = new AppAutomate(_androidPercyAppiumDriver.Object);
      var result = appAutomate.ExecutePercyScreenshotBegin(name);
      string actual = result.GetValue("success").ToString();
      // Then
      Assert.Equal(actual, "True");
      _androidPercyAppiumDriver.Verify(x => x.ExecuteScript("browserstack_executor:" + obj.ToString()), Times.Once);
    }

    [Fact]
    public void TestExecutePercyScreenshotBegin_WhenThrowError()
    {
      // Given
      var arguments = new JObject();
      var name = "First";
      var reqObject = JObject.FromObject(new
      {
        action = "percyScreenshot",
        arguments = new
        {
          state = "begin",
          percyBuildId = Environment.GetEnvironmentVariable("PERCY_BUILD_ID"),
          percyBuildUrl = Environment.GetEnvironmentVariable("PERCY_BUILD_URL"),
          name = name
        }
      });
      _androidPercyAppiumDriver.Setup(x => x.ExecuteScript("browserstack_executor:" + reqObject.ToString()))
        .Throws(new Exception());
      // When
      var appAutomate = new AppAutomate(_androidPercyAppiumDriver.Object);
      // Then
      Assert.Throws<NullReferenceException>(() => appAutomate.ExecutePercyScreenshotBegin(name).GetValue("success").ToString());
    }

    [Fact]
    public void TestExecutePercyScreenshotEnd()
    {
      // Given
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
          name = name
        }
      });

      _androidPercyAppiumDriver.Setup(x => x.ExecuteScript("browserstack_executor:" + reqObject.ToString()))
        .Returns(response.ToString());
      // When
      var appAutomate = new AppAutomate(_androidPercyAppiumDriver.Object);
      var result = appAutomate.ExecutePercyScreenshotEnd(name, percyScreenshotUrl, null);
      var actual = result.GetValue("success").ToString();
      // Then
      Assert.Equal(actual, "True");
      _androidPercyAppiumDriver.Verify(x => x.ExecuteScript("browserstack_executor:" + reqObject.ToString()), Times.Once);
    }

    [Fact]
    public void TestExecutePercyScreenshotEnd_WhenError()
    {
      // Given
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
          name = name
        }
      });
      _androidPercyAppiumDriver.Setup(x => x.ExecuteScript("browserstack_executor:" + reqObject.ToString()))
        .Returns(response.ToString());
      // When
      var appAutomate = new AppAutomate(_androidPercyAppiumDriver.Object);
      string actual = appAutomate.ExecutePercyScreenshotEnd(name, percyScreenshotUrl, "some error").GetValue("success").ToString();
      // Then
      Assert.Equal(actual, "False");
      _androidPercyAppiumDriver.Verify(x => x.ExecuteScript("browserstack_executor:" + reqObject.ToString()), Times.Once);
    }

    [Fact]
    public void TestExecutePercyScreenshotEnd_WhenException()
    {
      // Given
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
      // When
      var app = new AppAutomate(_androidPercyAppiumDriver.Object);
      // Then
      Assert.Throws<NullReferenceException>(() => app.ExecutePercyScreenshotEnd(null, null, null).GetValue("success").ToString());
      _androidPercyAppiumDriver.Verify(x => x.ExecuteScript("browserstack_executor:" + reqObject.ToString()), Times.Never);
    }

    [Fact]
    public void CaptureTiles_ShouldReturnListOfTiles_WhenCalled()
    {
      // Arrange
      var androidPercyAppiumDriver = new Mock<IPercyAppiumDriver>();
      var viewport = new Dictionary<string, object>(){
        {"top", 10L}
      };
      capabilities.Setup(x => x.GetCapability("platformName"))
        .Returns("Android");
      capabilities.Setup(x => x.GetCapability("platformVersion"))
        .Returns("9");
      capabilities.Setup(x => x.GetCapability("os_version"))
        .Returns("9");
      capabilities.Setup(x => x.GetCapability("deviceScreenSize"))
        .Returns("1280x1420");
      capabilities.Setup(x => x.GetCapability("orientation"))
        .Returns("landscape");
      capabilities.Setup(x => x.GetCapability("viewportRect"))
        .Returns(viewport);
      _androidPercyAppiumDriver.Setup(x => x.GetCapabilities())
        .Returns(capabilities.Object);
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
      Environment.SetEnvironmentVariable("PERCY_LOGLEVEL", "debug");
      var response = JsonConvert.SerializeObject(new
      {
        success = true,
        result = JsonConvert.SerializeObject(new List<object> {
            new { sha = "abcd-1234", header_height = 50, footer_height = 30 }
          })
      }
      );
      var capabilities = new Mock<ICapabilities>();
      capabilities.Setup(x => x.GetCapability("deviceScreenSize"))
        .Returns("1280x1420");
      _androidPercyAppiumDriver.Setup(x => x.GetCapabilities())
        .Returns(capabilities.Object);
      _androidPercyAppiumDriver.Setup(x => x.ExecuteScript(It.IsAny<string>())).Returns(response);

      var options = new ScreenshotOptions();
      options.ScreenLengths = 2;
      options.ScrollableXpath = "xapth/dummy/scrollable";
      // When
      var appAutomate = new AppAutomate(_androidPercyAppiumDriver.Object);
      var metadata = new AndroidMetadata(_androidPercyAppiumDriver.Object, "Samsung Galaxy s22", 100, 200, null, null);
      appAutomate.metadata = metadata;
      var actual = appAutomate.ExecutePercyScreenshot(options);
      // Then
      _androidPercyAppiumDriver.Verify(x => x.ExecuteScript(It.IsAny<string>()), Times.Once);
      Assert.Contains("abcd-1234", actual);
    }

    [Fact]
    public void TestDeviceName_WhenValueIsNull()
    {
      // Given
      var json = @"{deviceName:'Samsung Galaxy S22'}";
      var result = JObject.Parse(json);
      var expected = "Samsung Galaxy S22";
      // When
      var appAutomate = new AppAutomate(_androidPercyAppiumDriver.Object);
      var actual = appAutomate.DeviceName(null, result);
      // Then
      Assert.Equal(actual, expected);
    }

    [Fact]
    public void TestDeviceName_WhenProvidedInParams()
    {
      // Given
      var json = @"{deviceName:'Samsung Galaxy S22'}";
      var result = JObject.Parse(json);
      var expected = "Samsung Galaxy S21";
      // When
      var appAutomate = new AppAutomate(_androidPercyAppiumDriver.Object);
      var actual = appAutomate.DeviceName(expected, result);
      // Then
      Assert.Equal(actual, expected);
    }
  }
}
