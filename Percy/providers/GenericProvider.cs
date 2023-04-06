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

    public virtual String Screenshot(String name, ScreenshotOptions options ,String platformVersion = null)
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
      var ignoredElementLocation = IgnoredElementsLocation(options);
      var tiles = CaptureTiles(options);
      return CliWrapper.PostScreenshot(name, tag, tiles, debugUrl, ignoredElementLocation);
    }

    public JObject IgnoredElementsLocation(ScreenshotOptions options)
    {
      var ignoredElementsArray = new JArray();
      IgnoreLocationByXpaths(ignoredElementsArray, options.Xpaths);
      IgnoreLocationByIds(ignoredElementsArray, options.AccessibilityIds);
      IgnoreLocationByElement(ignoredElementsArray, options.AppiumElements);
      AddCustomIgnoreLocation(ignoredElementsArray, options.CustomIgnoreRegions);

      var ignoredElementsLocations = JObject.FromObject(new
      {
        ignoreElementsData = ignoredElementsArray
      });
      
      return ignoredElementsLocations;
    }

    public JObject IgnoreElementObject(String selector, Point location, Size size)
    {
      return JObject.FromObject(new
      {
        selector = selector,
        co_ordinates = new
        {
          top = location.Y,
          bottom = location.Y + size.Height,
          left = location.X,
          right = location.X + size.Width
        }
      });     
    }

    public void IgnoreLocationByXpaths(JArray ignoredElementsArray, List<String> xpaths)
    {
      var index = 0;
      foreach (var xpath in xpaths)
      {
        try
        {
          var element =  percyAppiumDriver.FindElementByXPath(xpath);
          
          var location = element.Location;
          var size = element.Size;
          var selector = string.Format("xpath {0} {1}", index, xpath);
          var ignoredRegion = IgnoreElementObject(selector, location, size);
          ignoredElementsArray.Add(ignoredRegion);
        } catch(Exception e) {
          AppPercy.Log("Appium Element with xpath:" + xpath + " not found. Ignoring this xpath.");
          AppPercy.Log(e.ToString());
        }
        index++;
      }
    }

    public void IgnoreLocationByIds(JArray ignoredElementsArray, List<String> ids)
    {
      var index = 0;
      foreach (var id in ids)
      {
        try
        {
          var element = percyAppiumDriver.FindElementsByAccessibilityId(id);

          var location = element.Location;
          var size = element.Size;
          var selector = string.Format("id {0} {1}", index, id);
          var ignoredRegion = IgnoreElementObject(selector, location, size);
          ignoredElementsArray.Add(ignoredRegion);
        } catch (Exception e) {
          AppPercy.Log("Appium Element with id:" + id + " not found. Ignoring this id.");
          AppPercy.Log(e.ToString());
        }
        index++;
      }
    }

    public void IgnoreLocationByElement(JArray ignoredElementsArray, List<AppiumWebElement> elements)
    {
      var index = 0;
      foreach (var element in elements)
      {
        try
        {
          var location = element.Location;
          var size = element.Size;
          string type = element.GetAttribute("class");
          var selector = string.Format("element {0} {1}", index, type);
            
          var ignoredRegion = IgnoreElementObject(selector, location, size);
          ignoredElementsArray.Add(ignoredRegion);
        } catch (Exception e) {
          AppPercy.Log("Correct Appium Element not passed");
          AppPercy.Log(e.ToString(), "debug");
        }
        index++;
      }
    }

    public void AddCustomIgnoreLocation(JArray ignoredElementsArray, List<JObject> cusomLocations)
    {
      var index = 0;
      var width = metadata.DeviceScreenWidth();
      var height = metadata.DeviceScreenHeight();
      foreach (var customLocation in cusomLocations)
      {
        try
        {
          var top =  customLocation.GetValue("top");
          if (ValidateIgnoreLocation(customLocation)) {
            var selector = "custom ignore region " + index;
            var ignoredRegion = JObject.FromObject(new
            {
              selector = selector,
              co_ordinates = customLocation
            });
            ignoredElementsArray.Add(ignoredRegion);
          }
          else
            AppPercy.Log("Values passed in custom ignored region at index:- "+ index+ " is not valid");
        } catch (Exception e)
        {
          AppPercy.Log("Custom Ignore Region object not valid at index:- "+ index);
          AppPercy.Log(e.ToString(), "debug");
        }
        index++;
      }
    }

    public Boolean ValidateIgnoreLocation(JObject customLocation)
    {
      var width = metadata.DeviceScreenWidth();
      var height = metadata.DeviceScreenHeight();
      var top = customLocation.GetValue("top").ToObject<int>();
      var bottom = customLocation.GetValue("bottom").ToObject<int>();
      var left  = customLocation.GetValue("left").ToObject<int>();
      var right = customLocation.GetValue("right").ToObject<int>();

      if (top >= bottom || left >= right)
        return false;

      if (top < 0 || bottom < 0 || left < 0 || right < 0)
        return false;
      
      if (top >= height || bottom >= height || left >= width || right >= width)
        return false;
      return true;
    }
  }
}
