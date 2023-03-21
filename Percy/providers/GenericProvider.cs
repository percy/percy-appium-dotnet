using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Appium.iOS;

namespace PercyIO.Appium
{

  internal class GenericProvider
  {
    internal Metadata metadata;
    private string debugUrl = null;
    private IPercyAppiumDriver percyAppiumDriver;

    internal GenericProvider(IPercyAppiumDriver percyAppiumDriver)
    {
      this.percyAppiumDriver = percyAppiumDriver;
    }

    internal JObject GetTag()
    {
      var tag = new JObject();
      tag.Add("name", metadata.DeviceName());
      tag.Add("osName", metadata.OsName());
      tag.Add("osVersion", metadata.PlatformVersion());
      tag.Add("width", metadata.DeviceScreenWidth());
      tag.Add("height", metadata.DeviceScreenHeight());
      tag.Add("orientation", metadata.Orientation());
      return tag;
    }

    internal virtual List<Tile> CaptureTiles(Boolean fullScreen, bool fullPage, int? screenLengths)
    {
      if (fullPage)
      {
        AppPercy.Log("Full page screeshot is only supported on App Automate." +
          " Falling back to single page screenshot.");
      }
      var statusBar = metadata.StatBarHeight();
      var navBar = metadata.NavBarHeight();
      var srcString = CaptureScreenshot(percyAppiumDriver);
      var localFilePath = GetAbsolutePath(srcString);
      var headerHeight = 0;
      var footerHeight = 0;
      var tiles = new List<Tile>();
      tiles.Add(new Tile(localFilePath, statusBar, navBar, headerHeight, footerHeight, fullScreen));
      return tiles;
    }

    private String GetAbsolutePath(String srcString)
    {
      var dirPath = GetDirPath();
      var filePath = dirPath + System.Guid.NewGuid() + ".png";
      var imageBytes = Convert.FromBase64String(srcString);
      File.WriteAllBytes(filePath, imageBytes);
      return filePath;
    }

    private String GetDirPath()
    {
      var tempDir = Environment.GetEnvironmentVariable("PERCY_TMP_DIR") ?? null;
      if (tempDir == null)
      {
        tempDir = System.IO.Path.GetTempPath();
      }
      Directory.CreateDirectory(tempDir);
      return tempDir;
    }

    private string CaptureScreenshot(IPercyAppiumDriver driver)
    {
      return driver.GetScreenshot().AsBase64EncodedString;
    }

    internal void SetDebugUrl(string debugUrl)
    {
      this.debugUrl = debugUrl;
    }

    public virtual String Screenshot(String name, String deviceName, int statusBarHeight, int navBarHeight,
        String orientation, Boolean fullScreen, bool fullPage, List<String> xpaths, List<String> ids,
        List<AppiumWebElement> elements,
        int? screenLengths, String platformVersion = null)
    {
      this.metadata = MetadataHelper.Resolve(
        percyAppiumDriver,
        deviceName,
        statusBarHeight,
        navBarHeight,
        orientation,
        platformVersion
      );
      var tag = GetTag();
      var ignoredElementLocation = IgnoredElementsLocation(xpaths, ids, elements);
      Console.WriteLine(ignoredElementLocation.ToString());
      var tiles = CaptureTiles(fullScreen, fullPage, screenLengths);
      return CliWrapper.PostScreenshot(name, tag, tiles, debugUrl, ignoredElementLocation);
    }

    public JObject IgnoredElementsLocation(List<String> xpaths, List<String> ids,
        List<AppiumWebElement> elements)
    {
      var ignoredElementsArray = new JArray();
      var allElements = new List<AppiumWebElement>();


      foreach (var xpath in xpaths)
      {
        var element =  percyAppiumDriver.FindElementByXPath(xpath);
        
        var location = element.Location;
        var size = element.Size;
        var ignoredRegion = new JObject(
          new JProperty("selector", "xpaths"),
          new JProperty("co-ordinates", new JObject(
              new JProperty("top", location.Y),
              new JProperty("bottom", location.Y + size.Height),
              new JProperty("left", location.X),
              new JProperty("right", location.X + size.Width)
            )
          )
        );
        ignoredElementsArray.Add(ignoredRegion);
        
      }

      foreach (var id in ids)
      {
        Console.WriteLine("INSIDE IDS");
        var element = percyAppiumDriver.FindElementsByAccessibilityId(id);

        Console.WriteLine("Loops");
        var location = element.Location;
        var size = element.Size;
        var ignoredRegion = new JObject(
          new JProperty("selector", "ids"),
          new JProperty("co-ordinates", new JObject(
              new JProperty("top", location.Y),
              new JProperty("bottom", location.Y + size.Height),
              new JProperty("left", location.X),
              new JProperty("right", location.X + size.Width)
            )
          )
        );
        ignoredElementsArray.Add(ignoredRegion);
      }

      foreach (var element in elements)
      {
        var location = element.Location;
        var size = element.Size;
        var ignoredRegion = new JObject(
          new JProperty("selector", "appiumWebElement"),
          new JProperty("co-ordinates", new JObject(
              new JProperty("top", location.Y),
              new JProperty("bottom", location.Y + size.Height),
              new JProperty("left", location.X),
              new JProperty("right", location.X + size.Width)
            )
          )
        );
        ignoredElementsArray.Add(ignoredRegion);
        
      }

      var ignoredElements = new JObject(
        new JProperty("ignore-element-data", ignoredElementsArray)
      );

      return ignoredElements;
    }
  }
}
