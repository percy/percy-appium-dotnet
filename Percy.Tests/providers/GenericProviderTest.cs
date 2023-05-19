using System;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium;
using Newtonsoft.Json.Linq;
using Moq;
using Xunit;
using PercyIO.Appium;
using RichardSzalay.MockHttp;
using System.Net.Http;
using System.Collections.Generic;
using OpenQA.Selenium.Appium;
using System.Drawing;
using OpenQA.Selenium.Appium.Interfaces;
using OpenQA.Selenium.Appium.Android;
using System.Reflection;

namespace Percy.Tests
{
  public class GenericProviderTest
  {
    private readonly AppAutomate appAutomate;
    private AndroidMetadata androidMetadata;
    private readonly Mock<IPercyAppiumDriver> _androidPercyAppiumDriver = new Mock<IPercyAppiumDriver>();
    private Mock<ICapabilities> capabilities = new Mock<ICapabilities>();

    public GenericProviderTest()
    {
      _androidPercyAppiumDriver.Setup(x => x.sessionId())
        .Returns(new SessionId("abc").ToString());
      _androidPercyAppiumDriver.Setup(x => x.GetType())
        .Returns("Android");
    }

    // flaky
    [Fact]
    public void TestGetTag()
    {
      // Given
      AppPercy.cache.Clear();
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
      _androidPercyAppiumDriver.Setup(x => x.GetCapabilities())
        .Returns(capabilities.Object);
      // When
      var genericProvider = new GenericProvider(_androidPercyAppiumDriver.Object);
      var metadata = MetadataHelper.Resolve(_androidPercyAppiumDriver.Object, "Samsung Galaxy s22", 0, 0, null, null);
      genericProvider.metadata = metadata;
      // Then
      var tile = genericProvider.GetTag();
      Assert.Equal(tile.GetValue("name").ToString(), "Samsung Galaxy s22");
      Assert.Equal(tile.GetValue("osName").ToString(), "Android");
      Assert.Equal(tile.GetValue("osVersion").ToString(), "9");
      Assert.Equal(Convert.ToInt32(tile.GetValue("width")), 1280);
      Assert.Equal(Convert.ToInt32(tile.GetValue("height")), 1420);
      Assert.Equal(tile.GetValue("orientation").ToString(), "landscape");

    }

    [Fact]
    public void TestCaptureTile_WithoutFullScreen()
    {
      // Given
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
      var genericProvider = new GenericProvider(_androidPercyAppiumDriver.Object);
      var metadata = MetadataHelper.Resolve(_androidPercyAppiumDriver.Object, "Samsung Galaxy s22", 100, 200, null, null);
      genericProvider.metadata = metadata;
      var options = new ScreenshotOptions();
      options.FullScreen = false;
      options.FullPage = false;
      options.ScreenLengths = 0;
      // When
      var tile = genericProvider.CaptureTiles(options)[0];
      // Then
      Assert.True(tile.LocalFilePath.EndsWith(".png"));
      Assert.Equal(Convert.ToInt32(tile.StatusBarHeight), 100);
      Assert.Equal(Convert.ToInt32(tile.NavBarHeight), 200);
      Assert.Equal(Convert.ToInt32(tile.HeaderHeight), 0);
      Assert.Equal(Convert.ToInt32(tile.FooterHeight), 0);
      Assert.Equal(tile.FullScreen, false);
    }

    [Fact]
    public void TestCaptureTile_WithFullScreen()
    {
      // Given
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
      var genericProvider = new GenericProvider(_androidPercyAppiumDriver.Object);
      var metadata = MetadataHelper.Resolve(_androidPercyAppiumDriver.Object, "Samsung Galaxy s22", 100, 200, null, null);
      genericProvider.metadata = metadata;
      var options = new ScreenshotOptions();
      options.FullScreen = true;
      options.FullPage = false;
      options.ScreenLengths = 0;
      // When
      var tile = genericProvider.CaptureTiles(options)[0];
      // Then
      Assert.True(tile.LocalFilePath.EndsWith(".png"));
      Assert.Equal(Convert.ToInt32(tile.StatusBarHeight), 100);
      Assert.Equal(Convert.ToInt32(tile.NavBarHeight), 200);
      Assert.Equal(Convert.ToInt32(tile.HeaderHeight), 0);
      Assert.Equal(Convert.ToInt32(tile.FooterHeight), 0);
      Assert.Equal(tile.FullScreen, true);
    }

    [Fact]
    public void TestScreenshot()
    {
      // Given
      string expected = "https://percy.io/api/v1/comparisons/redirect?snapshot[name]=test%20screenshot&tag[name]=Samsung&tag[os_name]=Android&tag[os_version]=9&tag[width]=1280&tag[height]=1420&tag[orientation]=landscape";
      string url = "http://hub-cloud.browserstack.com/wd/hub";
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
      _androidPercyAppiumDriver.Setup(x => x.GetHost())
        .Returns(url);
      Screenshot screenshot = new Screenshot("c2hvcnRlc3Q=");
      _androidPercyAppiumDriver.Setup(x => x.GetScreenshot())
        .Returns(screenshot);
      var options = new ScreenshotOptions();
      options.DeviceName = "Samsung";
      options.StatusBarHeight = 0;
      options.NavBarHeight = 0;
      options.Orientation = "landscape";
      options.FullScreen = false;
      options.FullPage = false;
      options.ScreenLengths = 0;

      var mockHttp = new MockHttpMessageHandler();

      // Setup a respond for the user api (including a wildcard in the URL)
      mockHttp.When("http://localhost:5338/percy/comparison")
        .Respond("application/json", "{\"success\": true, \"link\": \"" + expected + "\"}");

      CliWrapper.setHttpClient(new HttpClient(mockHttp));
      GenericProvider genericProvider = new GenericProvider(_androidPercyAppiumDriver.Object);
      string s = genericProvider.Screenshot("test screenshot", options);
      Assert.Equal(expected, s);
      CliWrapper.resetHttpClient();
    }

    [Fact]
    public void TestCaptureTile_WithResloveThrowError()
    {
      // Given
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
      var genericProvider = new GenericProvider(_androidPercyAppiumDriver.Object);
      var metadata = MetadataHelper.Resolve(_androidPercyAppiumDriver.Object, "Samsung Galaxy s22", 100, 200, null, null);
      genericProvider.metadata = metadata;
      var options = new ScreenshotOptions();
      options.FullScreen = true;
      options.FullPage = false;
      options.ScreenLengths = 1;
      // When
      var tile = genericProvider.CaptureTiles(options)[0];
      // Then
      Assert.True(tile.LocalFilePath.EndsWith(".png"));
      Assert.Equal(Convert.ToInt32(tile.StatusBarHeight), 100);
      Assert.Equal(Convert.ToInt32(tile.NavBarHeight), 200);
      Assert.Equal(Convert.ToInt32(tile.HeaderHeight), 0);
      Assert.Equal(Convert.ToInt32(tile.FooterHeight), 0);
      Assert.Equal(tile.FullScreen, true);
    }

    [Fact]
    public void TestAddCustomIgnoreLocation_ValidLocation_AddsRegionToIgnoredElementsArray()
    {
      // Arrange
      var ignoredElementsArray = new JArray();
      var ignoreRegion = new IgnoreRegion(10, 100, 20, 200);
      var customLocations = new List<IgnoreRegion>
      {
        ignoreRegion
      };
      capabilities.Setup(x => x.GetCapability("deviceScreenSize"))
      .Returns("1280x1420");
      _androidPercyAppiumDriver.Setup(x => x.GetCapabilities())
      .Returns(capabilities.Object);
      var genericProvider = new GenericProvider(_androidPercyAppiumDriver.Object);
      var metadata = MetadataHelper.Resolve(_androidPercyAppiumDriver.Object, "Samsung Galaxy s22", 100, 200, null, null);
      genericProvider.metadata = metadata;
      // Act
      genericProvider.AddCustomIgnoreRegions(ignoredElementsArray, customLocations);

      // Assert
      Assert.Single(ignoredElementsArray);
      var ignoredRegion = ignoredElementsArray[0];
      Assert.Equal("custom ignore region 0", ignoredRegion["selector"].ToObject<string>());
      var co_ordinates = ignoredRegion["co_ordinates"];
      Assert.Equal(10, co_ordinates["top"].ToObject<int>());
      Assert.Equal(100, co_ordinates["bottom"].ToObject<int>());
      Assert.Equal(20, co_ordinates["left"].ToObject<int>());
      Assert.Equal(200, co_ordinates["right"].ToObject<int>());
    }

    [Fact]
    public void TestAddCustomIgnoreLocation_InvalidLocation_DoesNotAddRegionToIgnoredElementsArray()
    {
      // Arrange
      var ignoredElementsArray = new JArray();
      var ignoreRegion = new IgnoreRegion(0, 2000, 0, 600);
      var customLocations = new List<IgnoreRegion>
      {
        ignoreRegion
      };
      capabilities.Setup(x => x.GetCapability("deviceScreenSize"))
      .Returns("1280x1420");
      _androidPercyAppiumDriver.Setup(x => x.GetCapabilities())
      .Returns(capabilities.Object);
      var genericProvider = new GenericProvider(_androidPercyAppiumDriver.Object);
      var metadata = MetadataHelper.Resolve(_androidPercyAppiumDriver.Object, "Samsung Galaxy s22", 100, 200, null, null);
      genericProvider.metadata = metadata;
      // Act
      genericProvider.AddCustomIgnoreRegions(ignoredElementsArray, customLocations);

      // Assert
      Assert.Empty(ignoredElementsArray);
    }
  }
}
