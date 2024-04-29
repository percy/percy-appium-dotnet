using System;
using System.Collections.Generic;

namespace PercyIO.Appium
{
  internal class IosMetadata : Metadata
  {
    private IPercyAppiumDriver driver;
    private String sessionId;

    internal IosMetadata(IPercyAppiumDriver driver, string deviceName, int statusBar, int navBar, string orientation, string platformVersion) : base(driver, deviceName, statusBar, navBar, orientation, platformVersion)
    {
      this.driver = driver;
      this.sessionId = driver.sessionId();
    }

    internal override string DeviceName()
    {
      var deviceName = GetDeviceName();
      if (deviceName != null)
      {
        return deviceName;
      }
      return driver.GetCapabilities().getValue<String>("deviceName")!;
    }

    internal override string OsName()
    {
      return "iOS";
    }

    internal override int DeviceScreenHeight()
    {
      var deviceScreenHeight = MetadataHelper.ValueFromStaticDevicesInfo("screenHeight",
              this.DeviceName().ToLower());
      if (deviceScreenHeight == 0)
      {
        return GetViewportRect().TryGetValue("height", out var value) ? (int)(long)value + StatBarHeight() : 0;
      }
      return deviceScreenHeight;
    }

    internal override int DeviceScreenWidth()
    {
      var deviceScreenWidth = MetadataHelper.ValueFromStaticDevicesInfo("screenWidth",
              this.DeviceName().ToLower());
      if (deviceScreenWidth == 0)
      {
        return GetViewportRect().TryGetValue("width", out var value) ? (int)(long)value : 0;
      }
      return deviceScreenWidth;
    }

    internal override int NavBarHeight()
    {
      var navBar = GetNavBar();
      return navBar != -1 ? navBar : 0;
    }

    internal override int StatBarHeight()
    {
      var statBar = GetStatusBar();
      if (statBar != -1)
      {
        return statBar;
      }
      var statBarHeight = MetadataHelper.ValueFromStaticDevicesInfo("statusBarHeight",
              this.DeviceName().ToLower());
      var pixelRatio = MetadataHelper.ValueFromStaticDevicesInfo("pixelRatio",
              this.DeviceName().ToLower());
      if (statBarHeight == 0)
      {
        return GetViewportRect().TryGetValue("top", out var value) ? (int)(long)value : 0;
      }
      return statBarHeight * pixelRatio;
    }

    private Dictionary<string, object> GetViewportRect()
    {
      if (AppPercy.cache.Get("viewportRect_" + sessionId) == null)
      {
        object viewportRect = driver.Execute("mobile: viewportRect");
        AppPercy.cache.Store("viewportRect_" + sessionId, viewportRect);
      }
      return (Dictionary<string, object>)AppPercy.cache.Get("viewportRect_" + sessionId);
    }

    internal override int ScaleFactor()
    {
      try
      {
        int actualWidth = (int)(long)GetViewportRect()["width"];
        int width = driver.DownscaledWidth();
        return (int)(long)(actualWidth/width);
      }
      catch (Exception e)
      {
        Utils.Log("Failed to get scale factor, full page screenshot might look incorrect");
        return 1;
      }
    }
  }
}
