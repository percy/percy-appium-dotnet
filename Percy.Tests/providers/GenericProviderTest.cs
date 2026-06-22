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

    // Targets GenericProvider.cs lines 34-38: the full-page fallback warning branch.
    // The default mock driver's GetHost() is not a BrowserStack hub, so
    // AppAutomate.Supports(...) is false and the warning log branch executes.
    [Fact]
    public void TestCaptureTile_FullPageOnNonAppAutomate_FallsBackToSinglePage()
    {
      // Arrange
      AppPercy.cache.Clear();
      _androidPercyAppiumDriver.Setup(x => x.GetHost())
        .Returns("http://hub-cloud.abc.com/wd/hub");
      var genericProvider = new GenericProvider(_androidPercyAppiumDriver.Object);
      var metadata = MetadataHelper.Resolve(_androidPercyAppiumDriver.Object, "Samsung Galaxy s22", 100, 200, null, null);
      genericProvider.metadata = metadata;
      var options = new ScreenshotOptions();
      options.FullScreen = false;
      options.FullPage = true;
      options.ScreenLengths = 0;
      // Act
      var tiles = genericProvider.CaptureTiles(options);
      // Assert — falls back to a single tile despite FullPage being requested
      Assert.Single(tiles);
      Assert.True(tiles[0].LocalFilePath.EndsWith(".png"));
      Assert.Equal(100, Convert.ToInt32(tiles[0].StatusBarHeight));
      Assert.Equal(200, Convert.ToInt32(tiles[0].NavBarHeight));
    }

    // Targets lines 126-141 (RegionObject) and 146-154 (RegionsByXpath happy path).
    // MockAppiumElement => Location (200,300), Size (100,200); Android ScaleFactor = 1.
    [Fact]
    public void TestRegionsByXpath_ValidElement_AddsRegionToElementsArray()
    {
      // Arrange
      AppPercy.cache.Clear();
      var element = new PercyAppiumElement(new MockAppiumElement());
      _androidPercyAppiumDriver.Setup(x => x.FindElementByXPath(It.IsAny<string>()))
        .Returns(element);
      var genericProvider = new GenericProvider(_androidPercyAppiumDriver.Object);
      var metadata = MetadataHelper.Resolve(_androidPercyAppiumDriver.Object, "Samsung Galaxy s22", 100, 200, null, null);
      genericProvider.metadata = metadata;
      var elementsArray = new JArray();
      var xpaths = new List<string> { "//android.widget.Button" };
      // Act
      genericProvider.RegionsByXpath(elementsArray, xpaths);
      // Assert
      Assert.Single(elementsArray);
      var region = elementsArray[0];
      Assert.Equal("xpath: //android.widget.Button", region["selector"].ToObject<string>());
      var coordinates = region["co_ordinates"];
      Assert.Equal(300, coordinates["top"].ToObject<int>());     // location.Y
      Assert.Equal(500, coordinates["bottom"].ToObject<int>());  // location.Y + size.Height
      Assert.Equal(200, coordinates["left"].ToObject<int>());    // location.X
      Assert.Equal(300, coordinates["right"].ToObject<int>());   // location.X + size.Width
    }

    // Targets lines 156-160: RegionsByXpath catch block (FindElementByXPath throws).
    [Fact]
    public void TestRegionsByXpath_ElementNotFound_SkipsXpath()
    {
      // Arrange
      AppPercy.cache.Clear();
      _androidPercyAppiumDriver.Setup(x => x.FindElementByXPath(It.IsAny<string>()))
        .Throws(new Exception("element not found"));
      var genericProvider = new GenericProvider(_androidPercyAppiumDriver.Object);
      var metadata = MetadataHelper.Resolve(_androidPercyAppiumDriver.Object, "Samsung Galaxy s22", 100, 200, null, null);
      genericProvider.metadata = metadata;
      var elementsArray = new JArray();
      var xpaths = new List<string> { "//does/not/exist" };
      // Act
      genericProvider.RegionsByXpath(elementsArray, xpaths);
      // Assert — missing element is ignored, nothing added
      Assert.Empty(elementsArray);
    }

    // Targets lines 166-174: RegionsByIds happy path (and RegionObject again).
    [Fact]
    public void TestRegionsByIds_ValidElement_AddsRegionToElementsArray()
    {
      // Arrange
      AppPercy.cache.Clear();
      var element = new PercyAppiumElement(new MockAppiumElement());
      _androidPercyAppiumDriver.Setup(x => x.FindElementsByAccessibilityId(It.IsAny<string>()))
        .Returns(element);
      var genericProvider = new GenericProvider(_androidPercyAppiumDriver.Object);
      var metadata = MetadataHelper.Resolve(_androidPercyAppiumDriver.Object, "Samsung Galaxy s22", 100, 200, null, null);
      genericProvider.metadata = metadata;
      var elementsArray = new JArray();
      var ids = new List<string> { "submit_button" };
      // Act
      genericProvider.RegionsByIds(elementsArray, ids);
      // Assert
      Assert.Single(elementsArray);
      var region = elementsArray[0];
      Assert.Equal("id: submit_button", region["selector"].ToObject<string>());
      var coordinates = region["co_ordinates"];
      Assert.Equal(300, coordinates["top"].ToObject<int>());
      Assert.Equal(500, coordinates["bottom"].ToObject<int>());
      Assert.Equal(200, coordinates["left"].ToObject<int>());
      Assert.Equal(300, coordinates["right"].ToObject<int>());
    }

    // Targets lines 176-180: RegionsByIds catch block.
    [Fact]
    public void TestRegionsByIds_ElementNotFound_SkipsId()
    {
      // Arrange
      AppPercy.cache.Clear();
      _androidPercyAppiumDriver.Setup(x => x.FindElementsByAccessibilityId(It.IsAny<string>()))
        .Throws(new Exception("element not found"));
      var genericProvider = new GenericProvider(_androidPercyAppiumDriver.Object);
      var metadata = MetadataHelper.Resolve(_androidPercyAppiumDriver.Object, "Samsung Galaxy s22", 100, 200, null, null);
      genericProvider.metadata = metadata;
      var elementsArray = new JArray();
      var ids = new List<string> { "missing_id" };
      // Act
      genericProvider.RegionsByIds(elementsArray, ids);
      // Assert
      Assert.Empty(elementsArray);
    }

    // Targets lines 186-195: RegionsByElements happy path. MockAppiumElement.Type()
    // (GetAttribute("class")) returns "Button"; selector => "element: 0 Button".
    [Fact]
    public void TestRegionsByElements_ValidElement_AddsRegionToElementsArray()
    {
      // Arrange
      AppPercy.cache.Clear();
      var genericProvider = new GenericProvider(_androidPercyAppiumDriver.Object);
      var metadata = MetadataHelper.Resolve(_androidPercyAppiumDriver.Object, "Samsung Galaxy s22", 100, 200, null, null);
      genericProvider.metadata = metadata;
      var elementsArray = new JArray();
      var elements = new List<object> { new MockAppiumElement() };
      // Act
      genericProvider.RegionsByElements(elementsArray, elements);
      // Assert
      Assert.Single(elementsArray);
      var region = elementsArray[0];
      Assert.Equal("element: 0 Button", region["selector"].ToObject<string>());
      var coordinates = region["co_ordinates"];
      Assert.Equal(300, coordinates["top"].ToObject<int>());
      Assert.Equal(500, coordinates["bottom"].ToObject<int>());
      Assert.Equal(200, coordinates["left"].ToObject<int>());
      Assert.Equal(300, coordinates["right"].ToObject<int>());
    }

    // Targets lines 197-201: RegionsByElements catch block. A non-element object
    // (no Location/Size/GetAttribute) makes PercyAppiumElement construction throw.
    [Fact]
    public void TestRegionsByElements_InvalidElement_SkipsElement()
    {
      // Arrange
      AppPercy.cache.Clear();
      var genericProvider = new GenericProvider(_androidPercyAppiumDriver.Object);
      var metadata = MetadataHelper.Resolve(_androidPercyAppiumDriver.Object, "Samsung Galaxy s22", 100, 200, null, null);
      genericProvider.metadata = metadata;
      var elementsArray = new JArray();
      var elements = new List<object> { "not an appium element" };
      // Act
      genericProvider.RegionsByElements(elementsArray, elements);
      // Assert — invalid element at index 0 is ignored
      Assert.Empty(elementsArray);
    }

    // Targets lines 236-240: RegionsByLocation catch block. A null entry makes
    // customLocations[index].IsValid(...) throw a NullReferenceException.
    [Fact]
    public void TestRegionsByLocation_ThrowingLocation_SkipsAndContinues()
    {
      // Arrange
      AppPercy.cache.Clear();
      var elementsArray = new JArray();
      var customLocations = new List<Region> { null };
      var genericProvider = new GenericProvider(_androidPercyAppiumDriver.Object);
      var metadata = MetadataHelper.Resolve(_androidPercyAppiumDriver.Object, "Samsung Galaxy s22", 100, 200, null, null);
      genericProvider.metadata = metadata;
      // Act — should swallow the exception and not add anything
      genericProvider.RegionsByLocation(elementsArray, customLocations);
      // Assert
      Assert.Empty(elementsArray);
    }

    // Targets the FindRegions aggregation path (lines 115-123) end-to-end:
    // xpath + accessibility id + appium element + custom location combine into one array.
    [Fact]
    public void TestFindRegions_AggregatesAllRegionTypes()
    {
      // Arrange
      AppPercy.cache.Clear();
      var element = new PercyAppiumElement(new MockAppiumElement());
      _androidPercyAppiumDriver.Setup(x => x.FindElementByXPath(It.IsAny<string>()))
        .Returns(element);
      _androidPercyAppiumDriver.Setup(x => x.FindElementsByAccessibilityId(It.IsAny<string>()))
        .Returns(element);
      var genericProvider = new GenericProvider(_androidPercyAppiumDriver.Object);
      var metadata = MetadataHelper.Resolve(_androidPercyAppiumDriver.Object, "Samsung Galaxy s22", 100, 200, null, null);
      genericProvider.metadata = metadata;
      var xpaths = new List<string> { "//android.widget.Button" };
      var ids = new List<string> { "submit_button" };
      var appiumElements = new List<object> { new MockAppiumElement() };
      var locations = new List<Region> { new Region(10, 100, 20, 200) };
      // Act
      var regions = genericProvider.FindRegions(xpaths, ids, appiumElements, locations);
      // Assert — one region from each of the four sources
      Assert.Equal(4, regions.Count);
      Assert.Equal("xpath: //android.widget.Button", regions[0]["selector"].ToObject<string>());
      Assert.Equal("id: submit_button", regions[1]["selector"].ToObject<string>());
      Assert.Equal("element: 0 Button", regions[2]["selector"].ToObject<string>());
      Assert.Equal("custom region 0", regions[3]["selector"].ToObject<string>());
    }
  }
}
