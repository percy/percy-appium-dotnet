using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;

namespace PercyIO.Appium
{

  internal class GenericProvider
  {
    private Metadata metadata;
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

    internal List<Tile> CaptureTiles(Boolean fullScreen)
    {
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
        String orientation, Boolean fullScreen, String platformVersion = null)
    {
      var tempMetadata = MetadataHelper.Resolve(percyAppiumDriver, deviceName, statusBarHeight, navBarHeight, orientation,
              platformVersion);
      setMetadata(tempMetadata);
      var tag = GetTag();
      var tiles = CaptureTiles(fullScreen);
      return CliWrapper.PostScreenshot(name, tag, tiles, debugUrl);
    }

    internal void setMetadata(Metadata metadata)
    {
      this.metadata = metadata;
    }
  }
}
