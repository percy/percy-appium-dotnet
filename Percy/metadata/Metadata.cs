using System;

namespace PercyIO.Appium
{
  internal abstract class Metadata
  {
    private String screenOrientation;
    private String platVersion;
    private int statusBar;
    private int navBar;
    private String device;
    private static IPercyAppiumDriver driver;

    internal Metadata(IPercyAppiumDriver driver, String deviceName, int statusBar, int navBar, String orientation,
        String platformVersion)
    {
      Metadata.driver = driver;
      this.platVersion = platformVersion;
      this.screenOrientation = orientation;
      this.statusBar = statusBar;
      this.navBar = navBar;
      this.device = deviceName;
    }

    internal String GetDeviceName()
    {
      return device;
    }

    internal int GetNavBar()
    {
      return navBar;
    }

    internal int GetStatusBar()
    {
      return statusBar;
    }

    internal string Orientation()
    {
      if (screenOrientation != null)
      {
        if (screenOrientation.ToLower().Equals("portrait") || screenOrientation.ToLower().Equals("landscape"))
        {
          return screenOrientation.ToLower();
        }
        else if (screenOrientation.ToLower().Equals("auto"))
        {
          return driver.Orientation().ToString().ToLower();
        }
        else
        {
          return "portrait";
        }
      }
      else
      {
        var orientationCapability = driver.GetCapabilities()?.getValue<String>("orientation");
        if (orientationCapability != null)
        {
          return orientationCapability.ToLower();
        }
        else
        {
          return "portrait";
        }
      }
    }

    internal string PlatformVersion()
    {
      if (platVersion != null)
      {
        return platVersion;
      }

      var osVersion = driver.GetCapabilities().getValue<String>("platformVersion");
      if (osVersion == null)
      {
        osVersion = driver.GetCapabilities().getValue<String>("os_version");
        if (osVersion == null)
        {
          return null;
        }
      }
      return osVersion;
    }

    internal abstract int DeviceScreenWidth();
    internal abstract String DeviceName();
    internal abstract int DeviceScreenHeight();
    internal abstract int StatBarHeight();
    internal abstract int NavBarHeight();
    internal abstract string OsName();
    internal abstract int ScaleFactor();
  }
}
