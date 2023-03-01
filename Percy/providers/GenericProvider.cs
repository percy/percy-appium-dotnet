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

    internal virtual List<Tile> CaptureTiles(Boolean fullScreen, bool fullPage, int? screenLengths)
    {
      if (fullPage) {
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
        String orientation, Boolean fullScreen, bool fullPage, int? screenLengths, String platformVersion = null)
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
      var tiles = CaptureTiles(fullScreen, fullPage, screenLengths);
      return CliWrapper.PostScreenshot(name, tag, tiles, debugUrl);
    }
  }
}
