using System;
using Newtonsoft.Json.Linq;
using Moq;
using Xunit;
using PercyIO.Appium;
using RichardSzalay.MockHttp;
using System.Net.Http;
using System.Collections.Generic;

namespace Percy.Tests
{
  public class GenericProviderTest
  {
    private readonly Mock<IPercyAppiumDriver> _androidPercyAppiumDriver = new Mock<IPercyAppiumDriver>();

    public GenericProviderTest()
    {
      _androidPercyAppiumDriver = MetadataBuilder.mockDriver("Android");
    }

    [Fact]
    public void TestGetTag()
    {
      // Arrange
      AppPercy.cache.Clear();
      // Act
      var genericProvider = new GenericProvider(_androidPercyAppiumDriver.Object);
      var metadata = MetadataHelper.Resolve(_androidPercyAppiumDriver.Object, "Samsung Galaxy s22", 0, 0, null, null);
      genericProvider.metadata = metadata;
      // Assert
      var tile = genericProvider.GetTag();
      Assert.Equal(tile.GetValue("name").ToString(), "Samsung Galaxy s22");
      Assert.Equal(tile.GetValue("osName").ToString(), "Android");
      Assert.Equal(tile.GetValue("osVersion").ToString(), "12.0");
      Assert.Equal(Convert.ToInt32(tile.GetValue("width")), 1280);
      Assert.Equal(Convert.ToInt32(tile.GetValue("height")), 1420);
      Assert.Equal(tile.GetValue("orientation").ToString(), "portrait");

    }

    [Fact]
    public void TestCaptureTile_WithoutFullScreen()
    {
      // Arrange
      AppPercy.cache.Clear();
      var genericProvider = new GenericProvider(_androidPercyAppiumDriver.Object);
      var metadata = MetadataHelper.Resolve(_androidPercyAppiumDriver.Object, "Samsung Galaxy s22", 100, 200, null, null);
      genericProvider.metadata = metadata;
      var options = new ScreenshotOptions();
      options.FullScreen = false;
      options.FullPage = false;
      options.ScreenLengths = 0;
      // Act
      var tile = genericProvider.CaptureTiles(options)[0];
      // Assert
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
      // Arrange
      AppPercy.cache.Clear();
      var genericProvider = new GenericProvider(_androidPercyAppiumDriver.Object);
      var metadata = MetadataHelper.Resolve(_androidPercyAppiumDriver.Object, "Samsung Galaxy s22", 100, 200, null, null);
      genericProvider.metadata = metadata;
      var options = new ScreenshotOptions();
      options.FullScreen = true;
      options.FullPage = false;
      options.ScreenLengths = 0;
      // Act
      var tile = genericProvider.CaptureTiles(options)[0];
      // Assert
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
      // Arrange
      string expected = "https://percy.io/api/v1/comparisons/redirect?snapshot[name]=test%20screenshot&tag[name]=Samsung&tag[os_name]=Android&tag[os_version]=9&tag[width]=1280&tag[height]=1420&tag[orientation]=landscape";
      string url = "http://hub-cloud.browserstack.com/wd/hub";
      AppPercy.cache.Clear();
      _androidPercyAppiumDriver.Setup(x => x.GetHost())
        .Returns(url);
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
      // Act
      GenericProvider genericProvider = new GenericProvider(_androidPercyAppiumDriver.Object);
      var s = genericProvider.Screenshot("test screenshot", options);
      // Assert
      Assert.Equal(expected, s.GetValue("link"));
      CliWrapper.resetHttpClient();
    }

    [Fact]
    public void TestCaptureTile_WithResloveThrowError()
    {
      // Arrange
      AppPercy.cache.Clear();
      var genericProvider = new GenericProvider(_androidPercyAppiumDriver.Object);
      var metadata = MetadataHelper.Resolve(_androidPercyAppiumDriver.Object, "Samsung Galaxy s22", 100, 200, null, null);
      genericProvider.metadata = metadata;
      var options = new ScreenshotOptions();
      options.FullScreen = true;
      options.FullPage = false;
      options.ScreenLengths = 1;
      // Act
      var tile = genericProvider.CaptureTiles(options)[0];
      // Assert
      Assert.True(tile.LocalFilePath.EndsWith(".png"));
      Assert.Equal(Convert.ToInt32(tile.StatusBarHeight), 100);
      Assert.Equal(Convert.ToInt32(tile.NavBarHeight), 200);
      Assert.Equal(Convert.ToInt32(tile.HeaderHeight), 0);
      Assert.Equal(Convert.ToInt32(tile.FooterHeight), 0);
      Assert.Equal(tile.FullScreen, true);
    }

    [Fact]
    public void TestRegionsByLocation_ValidLocation_AddsRegionElementsArray()
    {
      // Arrange
      var elementsArray = new JArray();
      var customRegion = new Region(10, 100, 20, 200);
      Region invaliCustomRegion1 = new Region(1400, 1500, 200, 250);
      Region invaliCustomRegion2 = new Region(50, 100, 1270, 1300);
      var customLocations = new List<Region>
      {
        customRegion,
        invaliCustomRegion1,
        invaliCustomRegion2
      };
      var genericProvider = new GenericProvider(_androidPercyAppiumDriver.Object);
      var metadata = MetadataHelper.Resolve(_androidPercyAppiumDriver.Object, "Samsung Galaxy s22", 100, 200, null, null);
      genericProvider.metadata = metadata;
      Assert.Equal(1280, metadata.DeviceScreenWidth());
      Assert.Equal(1420, metadata.DeviceScreenHeight());
      // Act
      genericProvider.RegionsByLocation(elementsArray, customLocations);

      // Assert
      Assert.Single(elementsArray);
      var region = elementsArray[0];
      Assert.Equal("custom region 0", region["selector"].ToObject<string>());
      var co_ordinates = region["co_ordinates"];
      Assert.Equal(10, co_ordinates["top"].ToObject<int>());
      Assert.Equal(100, co_ordinates["bottom"].ToObject<int>());
      Assert.Equal(20, co_ordinates["left"].ToObject<int>());
      Assert.Equal(200, co_ordinates["right"].ToObject<int>());
    }

    [Fact]
    public void TestRegionsByLocation_InvalidLocation_DoesNotAddRegionToElementsArray()
    {
      // Arrange
      var elementsArray = new JArray();
      var customRegion = new Region(0, 2000, 0, 600);
      var customLocations = new List<Region>
      {
        customRegion
      };
      var genericProvider = new GenericProvider(_androidPercyAppiumDriver.Object);
      var metadata = MetadataHelper.Resolve(_androidPercyAppiumDriver.Object, "Samsung Galaxy s22", 100, 200, null, null);
      genericProvider.metadata = metadata;
      // Act
      genericProvider.RegionsByLocation(elementsArray, customLocations);

      // Assert
      Assert.Empty(elementsArray);
    }
  }
}
