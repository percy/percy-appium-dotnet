using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium.Appium;

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

    internal virtual List<Tile> CaptureTiles(ScreenshotOptions options)
    {
      if (options.FullPage)
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
      tiles.Add(new Tile(localFilePath, statusBar, navBar, headerHeight, footerHeight, options.FullScreen));
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

    public virtual String Screenshot(String name, ScreenshotOptions options, String platformVersion = null)
    {
      this.metadata = MetadataHelper.Resolve(
        percyAppiumDriver,
        options.DeviceName,
        options.StatusBarHeight,
        options.NavBarHeight,
        options.Orientation,
        platformVersion
      );
      var tag = GetTag();
      var ignoredElementLocation = FindIgnoredRegions(options);
      var tiles = CaptureTiles(options);
      return CliWrapper.PostScreenshot(name, tag, tiles, debugUrl, ignoredElementLocation);
    }

    public JObject FindIgnoredRegions(ScreenshotOptions options)
    {
      var ignoredElementsArray = new JArray();
      IgnoreRegionsByXpaths(ignoredElementsArray, options.IgnoreRegionXpaths);
      IgnoreRegionsByIds(ignoredElementsArray, options.IgnoreRegionAccessibilityIds);
      IgnoreRegionsByElement(ignoredElementsArray, options.IgnoreRegionAppiumElements);
      AddCustomIgnoreRegions(ignoredElementsArray, options.CustomIgnoreRegions);

      var ignoredElementsLocations = JObject.FromObject(new
      {
        ignoreElementsData = ignoredElementsArray
      });

      return ignoredElementsLocations;
    }

    public JObject IgnoreElementObject(String selector, Point location, Size size)
    {
      var scaleFactor = metadata.ScaleFactor();
      return JObject.FromObject(new
      {
        selector = selector,
        co_ordinates = new
        {
          top = location.Y * scaleFactor,
          bottom = (location.Y + size.Height) * scaleFactor,
          left = location.X * scaleFactor,
          right = (location.X + size.Width) * scaleFactor
        }
      });
    }

    public void IgnoreRegionsByXpaths(JArray ignoredElementsArray, List<String> xpaths)
    {
      foreach (var xpath in xpaths)
      {
        try
        {
          var element = percyAppiumDriver.FindElementByXPath(xpath);

          var location = element.Location;
          var size = element.Size;
          var selector = string.Format("xpath: {0}", xpath);
          var ignoredRegion = IgnoreElementObject(selector, location, size);
          ignoredElementsArray.Add(ignoredRegion);
        }
        catch (Exception e)
        {
          AppPercy.Log("Appium Element with xpath:" + xpath + " not found. Ignoring this xpath.");
          AppPercy.Log(e.ToString());
        }
      }
    }

    public void IgnoreRegionsByIds(JArray ignoredElementsArray, List<String> ids)
    {
      foreach (var id in ids)
      {
        try
        {
          var element = percyAppiumDriver.FindElementsByAccessibilityId(id);

          var location = element.Location;
          var size = element.Size;
          var selector = string.Format("id: {0}", id);
          var ignoredRegion = IgnoreElementObject(selector, location, size);
          ignoredElementsArray.Add(ignoredRegion);
        }
        catch (Exception e)
        {
          AppPercy.Log("Appium Element with id:" + id + " not found. Ignoring this id.");
          AppPercy.Log(e.ToString());
        }
      }
    }

    public void IgnoreRegionsByElement(JArray ignoredElementsArray, List<AppiumWebElement> elements)
    {
      for (var index = 0; index < elements.Count; index++)
      {
        try
        {
          var location = elements[index].Location;
          var size = elements[index].Size;
          string type = elements[index].GetAttribute("class");
          var selector = string.Format("element: {0} {1}", index, type);

          var ignoredRegion = IgnoreElementObject(selector, location, size);
          ignoredElementsArray.Add(ignoredRegion);
        }
        catch (Exception e)
        {
          AppPercy.Log("Correct Appium Element not passed at index " + index + ".");
          AppPercy.Log(e.ToString(), "debug");
        }
      }
    }

    public void AddCustomIgnoreRegions(JArray ignoredElementsArray, List<IgnoreRegion> customLocations)
    {
      var width = metadata.DeviceScreenWidth();
      var height = metadata.DeviceScreenHeight();
      for (var index = 0; index < customLocations.Count; index++)
      {
        try
        {
          if (customLocations[index].IsValid(width, height))
          {
            var selector = "custom ignore region " + index;
            var ignoredRegion = JObject.FromObject(new
            {
              selector = selector,
              co_ordinates = JObject.FromObject(new
              {
                top = customLocations[index].Top,
                bottom = customLocations[index].Bottom,
                left = customLocations[index].Left,
                right = customLocations[index].Right
              }
                
              )
            });
            ignoredElementsArray.Add(ignoredRegion);
          }
          else
          {
            AppPercy.Log("Values passed in custom ignored region at index:- " + index + " is not valid");
          }
        }
        catch (Exception e)
        {
          AppPercy.Log("Custom Ignore Region object not valid at index:- " + index);
          AppPercy.Log(e.ToString(), "debug");
        }
      }
    }
  }
}
