using System.Collections.Generic;
using System;

namespace PercyIO.Appium
{
  internal class AndroidMetadata : Metadata
  {
    private IPercyAppiumDriver driver;
    private String sessionId;

    internal AndroidMetadata(IPercyAppiumDriver driver, string deviceName, int statusBar, int navBar, string orientation, string platformVersion) :
    base(driver, deviceName, statusBar, navBar, orientation, platformVersion)
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
      var device = driver.GetCapabilities().getValue<String>("device");
      if (device == null)
      {
        Dictionary<string, object> desiredCaps = driver.GetCapabilities().getValue<Dictionary<string, object>>("desired");
        if (desiredCaps != null)
        {
          return desiredCaps.TryGetValue("deviceName", out var value) ? value.ToString() : "";
        }
        return "";
      }
      return device;
    }

    internal override int DeviceScreenHeight()
    {
      var deviceScreenSize = driver.GetCapabilities().getValue<String>("deviceScreenSize");
      if (deviceScreenSize == null) return 0;
      return Int16.Parse(deviceScreenSize.Split('x')[1]);
    }

    internal override string OsName()
    {
      return "Android";
    }
    internal override int DeviceScreenWidth()
    {
      var deviceScreenSize = driver.GetCapabilities().getValue<String>("deviceScreenSize");
      if (deviceScreenSize == null) return 0;
      return Int16.Parse(deviceScreenSize.Split('x')[0]);
    }

    internal override int NavBarHeight()
    {
      var navBar = GetNavBar();
      if (navBar != -1)
      {
        return navBar;
      }
      int fullDeviceScreenHeight = DeviceScreenHeight();
      int deviceScreenHeight = GetViewportRect().TryGetValue("height", out var value) ? (int)(long)value : 0;
      return fullDeviceScreenHeight - (deviceScreenHeight + StatBarHeight());
    }

    internal override int StatBarHeight()
    {
      var statBar = GetStatusBar();
      if (statBar != -1)
      {
        return statBar;
      }
      return GetViewportRect().TryGetValue("top", out var value) ? (int)(long)value : 0;
    }

    private Dictionary<string, object> GetViewportRect()
    {
      if (AppPercy.cache.Get("viewportRect_" + sessionId) == null)
      {
        var viewportRect = driver.GetCapabilities().getValue<Dictionary<string, object>>("viewportRect");
        if (viewportRect == null)
        {
          viewportRect = new Dictionary<string, object>();
        }
        AppPercy.cache.Store("viewportRect_" + sessionId, viewportRect);
      }
      return (Dictionary<string, object>)AppPercy.cache.Get("viewportRect_" + sessionId);
    }

    internal override int ScaleFactor()
    {
      return 1;
    }
  }
}
