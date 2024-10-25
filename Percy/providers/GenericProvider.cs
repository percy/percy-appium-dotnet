using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;

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
      if (options.FullPage && !AppAutomate.Supports(percyAppiumDriver))
      {
        Utils.Log("Full page screeshot is only supported on App Automate." +
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
      return driver.GetScreenshot();
    }

    internal void SetDebugUrl(string debugUrl)
    {
      this.debugUrl = debugUrl;
    }

    public virtual JObject Screenshot(String name, ScreenshotOptions options, String platformVersion = null)
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
      var ignoredRegions = FindRegions(
        options.IgnoreRegionXpaths,
        options.IgnoreRegionAccessibilityIds,
        options.IgnoreRegionAppiumElements,
        options.CustomIgnoreRegions
      );
      var consideredRegions = FindRegions(
        options.ConsiderRegionXpaths,
        options.ConsiderRegionAccessibilityIds,
        options.ConsiderRegionAppiumElements,
        options.CustomConsiderRegions
      );
      var ignoredElementsData = JObject.FromObject(new
      {
        ignoreElementsData = ignoredRegions
      });
      var consideredElementsData = JObject.FromObject(new
      {
        considerElementsData = consideredRegions
      });
      var tiles = CaptureTiles(options);
      return CliWrapper.PostScreenshot(name, tag, tiles, debugUrl, ignoredElementsData, consideredElementsData, options.Sync, options.TestCase, options.Labels, options.ThTestCaseExecutionId);
    }

    public JArray FindRegions(List<String> Xpaths, List<String> AccessibilityIds, List<Object> Elements, List<Region> Locations)
    {
      var elementsArray = new JArray();
      RegionsByXpath(elementsArray, Xpaths);
      RegionsByIds(elementsArray, AccessibilityIds);
      RegionsByElements(elementsArray, Elements);
      RegionsByLocation(elementsArray, Locations);

      return elementsArray;
    }

    public JObject RegionObject(String selector, PercyAppiumElement element)
    {
      var scaleFactor = metadata.ScaleFactor();
      var location = element.Location;
      var size = element.Size;
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

    public void RegionsByXpath(JArray elementsArray, List<String> xpaths)
    {
      foreach (var xpath in xpaths)
      {
        try
        {
          var element = percyAppiumDriver.FindElementByXPath(xpath);

          var selector = string.Format("xpath: {0}", xpath);
          var region = RegionObject(selector, element);
          elementsArray.Add(region);
        }
        catch (Exception e)
        {
          Utils.Log("Appium Element with xpath:" + xpath + " not found. Ignoring this xpath.");
          Utils.Log(e.ToString(), "debug");
        }
      }
    }

    public void RegionsByIds(JArray elementsArray, List<String> ids)
    {
      foreach (var id in ids)
      {
        try
        {
          var element = percyAppiumDriver.FindElementsByAccessibilityId(id);

          var selector = string.Format("id: {0}", id);
          var region = RegionObject(selector, element);
          elementsArray.Add(region);
        }
        catch (Exception e)
        {
          Utils.Log("Appium Element with id:" + id + " not found. Ignoring this id.");
          Utils.Log(e.ToString(), "debug");
        }
      }
    }

    public void RegionsByElements(JArray elementsArray, List<Object> elements)
    {
      for (var index = 0; index < elements.Count; index++)
      {
        try
        {
          var element = new PercyAppiumElement(elements[index]);
          string type = element.Type();
          var selector = string.Format("element: {0} {1}", index, type);

          var region = RegionObject(selector, element);
          elementsArray.Add(region);
        }
        catch (Exception e)
        {
          Utils.Log("Correct Appium Element not passed at index " + index + ".");
          Utils.Log(e.ToString(), "debug");
        }
      }
    }

    public void RegionsByLocation(JArray elementsArray, List<Region> customLocations)
    {
      var width = metadata.DeviceScreenWidth();
      var height = metadata.DeviceScreenHeight();
      for (var index = 0; index < customLocations.Count; index++)
      {
        try
        {
          if (customLocations[index].IsValid(height, width))
          {
            var selector = "custom region " + index;
            var region = JObject.FromObject(new
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
            elementsArray.Add(region);
          }
          else
          {
            Utils.Log("Values passed in custom region at index:- " + index + " is not valid");
          }
        }
        catch (Exception e)
        {
          Utils.Log("Custom Ignore Region object not valid at index:- " + index);
          Utils.Log(e.ToString(), "debug");
        }
      }
    }
  }
}
