using System;
using OpenQA.Selenium;
using Newtonsoft.Json.Linq;
using Moq;
using Xunit;
using PercyIO.Appium;
using System.Collections.Generic;

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
      appAutomate.SetDebugUrl(result);
      // Then
      Assert.Equal(appAutomate.GetDebugUrl(), expected);
    }

    [Fact]
    public void TestScreenshot()
    {
      // Given
      var response = @"{success:'true', osVersion:'11.2', buildHash:'abc', sessionHash:'def'}";
      _androidPercyAppiumDriver.Setup(x => x.ExecuteScript(It.IsAny<string>()))
        .Returns(response);
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
      // When
      AppAutomate appAutomate = new AppAutomate(_androidPercyAppiumDriver.Object);
      string actual = appAutomate.Screenshot("temp", "Samsung", 100, 100, "potrait", false);
      // Then
      Assert.Equal(actual,"");
    }

    [Fact]
    public void TestExecutePercyScreenshotBegin()
    {
      // Given
      var arguments = new JObject();
      string response = @"{success:'true'}";
      string name = "First";
      arguments.Add("state", "begin");
      arguments.Add("percyBuildId", Environment.GetEnvironmentVariable("PERCY_BUILD_ID"));
      arguments.Add("percyBuildUrl", Environment.GetEnvironmentVariable("PERCY_BUILD_URL"));
      arguments.Add("name", name);
      JObject reqObject = new JObject();
      reqObject.Add("action", "percyScreenshot");
      reqObject.Add("arguments", arguments);
      _androidPercyAppiumDriver.Setup(x => x.ExecuteScript("browserstack_executor:" + reqObject.ToString()))
        .Returns(response);
      // When
      AppAutomate appAutomate = new AppAutomate(_androidPercyAppiumDriver.Object);
      string actual = appAutomate.ExecutePercyScreenshotBegin(name).GetValue("success").ToString();
      // Then
      Assert.Equal(actual, "true");
      _androidPercyAppiumDriver.Verify(x => x.ExecuteScript("browserstack_executor:" + reqObject.ToString()), Times.Once);
    }

    [Fact]
    public void TestExecutePercyScreenshotBegin_WhenThrowError()
    {
      // Given
      var arguments = new JObject();
      var name = "First";
      arguments.Add("state", "begin");
      arguments.Add("percyBuildId", Environment.GetEnvironmentVariable("PERCY_BUILD_ID"));
      arguments.Add("percyBuildUrl", Environment.GetEnvironmentVariable("PERCY_BUILD_URL"));
      arguments.Add("name", name);
      var reqObject = new JObject();
      reqObject.Add("action", "percyScreenshot");
      reqObject.Add("arguments", arguments);
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
      var response = @"{success:'true'}";
      var name = "First";
      var percyScreenshotUrl = "";
      var arguments = new JObject();
      arguments.Add("state", "end");
      arguments.Add("percyScreenshotUrl", percyScreenshotUrl);
      arguments.Add("status", "success");
      arguments.Add("statusMessage", null);
      arguments.Add("name", name);
      var reqObject = new JObject();
      reqObject.Add("action", "percyScreenshot");
      reqObject.Add("arguments", arguments);
      _androidPercyAppiumDriver.Setup(x => x.ExecuteScript("browserstack_executor:" + reqObject.ToString()))
        .Returns(response);
      // When
      var appAutomate = new AppAutomate(_androidPercyAppiumDriver.Object);
      var actual = appAutomate.ExecutePercyScreenshotEnd(name, percyScreenshotUrl, null).GetValue("success").ToString();
      // Then
      Assert.Equal(actual, "true");
      _androidPercyAppiumDriver.Verify(x => x.ExecuteScript("browserstack_executor:" + reqObject.ToString()), Times.Once);
    }

    [Fact]
    public void TestExecutePercyScreenshotEnd_WhenError()
    {
      // Given
      Environment.SetEnvironmentVariable("PERCY_LOGLEVEL", "debug");
      var response = @"{success:'false'}";
      var name = "First";
      var percyScreenshotUrl = "";
      var arguments = new JObject();
      arguments.Add("state", "end");
      arguments.Add("percyScreenshotUrl", percyScreenshotUrl);
      arguments.Add("status", "failure");
      arguments.Add("statusMessage", "some error");
      arguments.Add("name", name);
      var reqObject = new JObject();
      reqObject.Add("action", "percyScreenshot");
      reqObject.Add("arguments", arguments);
      _androidPercyAppiumDriver.Setup(x => x.ExecuteScript("browserstack_executor:" + reqObject.ToString()))
        .Returns(response);
      // When
      var appAutomate = new AppAutomate(_androidPercyAppiumDriver.Object);
       string actual = appAutomate.ExecutePercyScreenshotEnd(name, percyScreenshotUrl, "some error").GetValue("success").ToString();
      // Then
      Assert.Equal(actual, "false");
      _androidPercyAppiumDriver.Verify(x => x.ExecuteScript("browserstack_executor:" + reqObject.ToString()), Times.Once);
    }

    [Fact]
    public void TestExecutePercyScreenshotEnd_WhenException()
    {
      // Given
      Environment.SetEnvironmentVariable("PERCY_LOGLEVEL", "debug");
      var name = "First";
      var percyScreenshotUrl = "";
      var arguments = new JObject();
      arguments.Add("state", "end");
      arguments.Add("percyScreenshotUrl", percyScreenshotUrl);
      arguments.Add("status", "failure");
      arguments.Add("statusMessage", "some error");
      arguments.Add("name", name);
      var reqObject = new JObject();
      reqObject.Add("action", "percyScreenshot");
      reqObject.Add("arguments", arguments);
      _androidPercyAppiumDriver.Setup(x => x.ExecuteScript("browserstack_executor:" + reqObject.ToString()))
        .Throws(new Exception());
      // When
      var app = new AppAutomate(_androidPercyAppiumDriver.Object);
      // Then
      Assert.Throws<NullReferenceException>(() => app.ExecutePercyScreenshotEnd(null, null, null).GetValue("success").ToString());
      _androidPercyAppiumDriver.Verify(x => x.ExecuteScript("browserstack_executor:" + reqObject.ToString()), Times.Never);
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

    [Fact]
    public void CaptureTiles_ShouldReturnListOfTiles_WhenCalled()
    {
      // Arrange
      var androidPercyAppiumDriver = new Mock<IPercyAppiumDriver>();
      var viewport = new Dictionary<string, object>(){
        {"top", 10L}
      };
      //var metadata = MetadataHelper.Resolve(_androidPercyAppiumDriver.Object, "Samsung Galaxy s22", 100, 200, null, null);
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
        .Returns("{'sha': 'abcd-1234', 'header_height': 50, 'footer_height': 30}");

      var appAutomate = new AppAutomate(_androidPercyAppiumDriver.Object);
      // Act
      var metadata = MetadataHelper.Resolve(_androidPercyAppiumDriver.Object, "Samsung Galaxy s22", 100, 200, null, null);
      appAutomate.setMetadata(metadata);
      var result = appAutomate.CaptureTiles(2, true, metadata,  "fullpage");

      // Assert
      Assert.IsType<System.Collections.Generic.List<Tile>>(result);
      Assert.Equal(1, result.Count);
      Assert.Equal("abcd", result[0].Sha);
      Assert.Equal(100, result[0].StatusBarHeight);
      Assert.Equal(200, result[0].NavBarHeight);
      Assert.Equal(50, result[0].HeaderHeight);
      Assert.Equal(30, result[0].FooterHeight);
    }

    [Fact]
    public void TestExecutePercyScreenshot() {
      Environment.SetEnvironmentVariable("PERCY_LOGLEVEL", "debug");
      var screenshotType = "fullpage";
      var response = @"{'sha': 'abcd-1234', 'header_height': 50, 'footer_height': 30}";
      var name = "First";
      var percyScreenshotUrl = "";
      var arguments = new JObject();
      arguments.Add("state", "end");
      arguments.Add("percyScreenshotUrl", percyScreenshotUrl);
      arguments.Add("status", "success");
      arguments.Add("statusMessage", null);
      arguments.Add("name", name);
      var reqObject = new JObject();
      reqObject.Add("action", "percyScreenshot");
      reqObject.Add("arguments", arguments);
     _androidPercyAppiumDriver.Setup(x => x.ExecuteScript(It.IsAny<string>()))
        .Returns(response);
      // When
      var appAutomate = new AppAutomate(_androidPercyAppiumDriver.Object);
      var metadata = MetadataHelper.Resolve(_androidPercyAppiumDriver.Object, "Samsung Galaxy s22", 100, 200, null, null);
      appAutomate.setMetadata(metadata);
      var actual = appAutomate.ExecutePercyScreenshot(screenshotType);
      // Then
      _androidPercyAppiumDriver.Verify(x => x.ExecuteScript(It.IsAny<string>()), Times.Once);
    }
  }
}
