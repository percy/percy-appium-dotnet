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
      return driver.GetCapabilities().GetCapability("deviceName").ToString();
    }

    internal override string OsName()
    {
      var osName = driver.GetCapabilities().GetCapability("platformName").ToString();
      return osName.Substring(0, 1).ToLower() + osName.Substring(1).ToUpper();
    }

    internal override int DeviceScreenHeight()
    {
      var deviceScreenHeight = MetadataHelper.ValueFromStaticDevicesInfo("screenHeight",
              this.DeviceName().ToLower());
      if (deviceScreenHeight == 0)
      {
        return GetViewportRect().TryGetValue("height", out var value) ? (int)(long)value : 0;
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
        object viewportRect;
        driver.GetSessionDetails().TryGetValue("viewportRect", out viewportRect);
        AppPercy.cache.Store("viewportRect_" + sessionId, viewportRect);
      }
      return (Dictionary<string, object>)AppPercy.cache.Get("viewportRect_" + sessionId);
    }

    internal override int ScaleFactor()
    {
      object scaleFactor;
      if (driver.GetSessionDetails().TryGetValue("pixelRatio", out scaleFactor))
      {
        return (int)(long) scaleFactor;
      }
      AppPercy.Log("Failed to get scale factor, full page screenshot might look incorrect");
      return 1;
    }
  }
}
