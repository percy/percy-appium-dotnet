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
      object device = driver.GetCapabilities().GetCapability("device");
      if (device == null)
      {
        Dictionary<string, object> desiredCaps = (Dictionary<string, object>)driver.GetCapabilities().GetCapability("desired");
        return desiredCaps.TryGetValue("deviceName", out var value) ? value.ToString() : "";
      }
      return device.ToString();
    }

    internal override int DeviceScreenHeight()
    {
      return Int16.Parse(driver.GetCapabilities().GetCapability("deviceScreenSize").ToString().Split('x')[1]);
    }

    internal override string OsName()
    {
      var osName = driver.GetCapabilities().GetCapability("platformName").ToString();
      return osName.Substring(0, 1).ToLower() + osName.Substring(1).ToUpper();
    }
    internal override int DeviceScreenWidth()
    {
      return Int16.Parse(driver.GetCapabilities().GetCapability("deviceScreenSize").ToString().Split('x')[0]);
    }

    internal override int NavBarHeight()
    {
      var navBar = GetNavBar();
      if (navBar != -1)
      {
        return navBar;
      }
      int fullDeviceScreenHeight = DeviceScreenHeight();
      int deviceScreenHeight = GetViewportRect().TryGetValue("top", out var value) ? (int)(long)value : 0;
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
        AppPercy.cache.Store("viewportRect_" + sessionId, driver.GetCapabilities().GetCapability("viewportRect"));
      }
      return (Dictionary<string, object>)AppPercy.cache.Get("viewportRect_" + sessionId);
    }

    internal override int GetScaleFactor()
    {
      return 1;
    }
  }
}
