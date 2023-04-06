using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Appium.iOS;
using System.Collections.Generic;
using OpenQA.Selenium.Appium;

namespace PercyIO.Appium
{
  internal class PercyAppiumDriver : IPercyAppiumDriver
  {
    private object driver;
    private String driverType;
    private IOSDriver<IOSElement>? iosDriver;
    private AndroidDriver<AndroidElement>? androidDriver;
    private AndroidDriver<AppiumWebElement>? appiumAndroidDriver;
    private IOSDriver<AppiumWebElement>? appiumIosDriver;

    internal PercyAppiumDriver(AndroidDriver<AndroidElement> driver)
    {
      this.driver = driver;
      this.driverType = "Android";
      this.androidDriver = driver;
    }

    internal PercyAppiumDriver(IOSDriver<AppiumWebElement> driver)
    {
      this.driver = driver;
      this.driverType = "iOS";
      this.appiumIosDriver = driver;
    }

    internal PercyAppiumDriver(AndroidDriver<AppiumWebElement> driver)
    {
      this.driver = driver;
      this.driverType = "Android";
      this.appiumAndroidDriver = driver;
    }

    internal PercyAppiumDriver(IOSDriver<IOSElement> driver)
    {
      this.driver = driver;
      this.driverType = "iOS";
      this.iosDriver = driver;
    }

    public new String GetType()
    {
      return driverType;
    }

    public String Orientation()
    {
      return iosDriver?.Orientation.ToString()!
        ?? androidDriver?.Orientation.ToString()!
        ?? appiumAndroidDriver?.Orientation.ToString()!
        ?? appiumIosDriver?.Orientation.ToString()!;
    }

    public ICapabilities GetCapabilities()
    {
      var key = "caps_" + sessionId();
      if (AppPercy.cache.Get(key) == null)
      {
        var caps = iosDriver?.Capabilities
          ?? androidDriver?.Capabilities
          ?? appiumAndroidDriver?.Capabilities
          ?? appiumIosDriver?.Capabilities;
        AppPercy.cache.Store(key, caps);
      }
      return (ICapabilities)AppPercy.cache.Get(key);
    }

    public IDictionary<string, object> GetSessionDetails()
    {

      var key = "session_" + sessionId();
      if (AppPercy.cache.Get(key) == null)
      {
        var sess = iosDriver?.SessionDetails 
          ?? androidDriver?.SessionDetails
          ?? appiumAndroidDriver?.SessionDetails
          ?? appiumIosDriver?.SessionDetails;
        AppPercy.cache.Store(key, sess);
      }
      return (IDictionary<string, object>)AppPercy.cache.Get(key);
    }

    public String sessionId()
    {
      return iosDriver?.SessionId?.ToString()!
        ?? androidDriver?.SessionId?.ToString()!
        ?? appiumAndroidDriver?.SessionId?.ToString()!
        ?? appiumIosDriver?.SessionId?.ToString()!;
    }

    public String ExecuteScript(String script)
    {
      return iosDriver?.ExecuteScript(script).ToString()!
        ?? androidDriver?.ExecuteScript(script).ToString()!
        ?? appiumAndroidDriver?.ExecuteScript(script).ToString()!
        ?? appiumIosDriver?.ExecuteScript(script).ToString()!;
    }

    public string GetHost()
    {
      Type type = driver.GetType();
      var property = type.GetProperty("CommandExecutor", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
      var commandExecutor = property?.GetValue(driver);
      var uri = commandExecutor?.GetType().GetField("URL", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
      var value = uri?.GetValue(commandExecutor);
      return value?.ToString()!;
    }

    public Screenshot GetScreenshot()
    {
      return iosDriver?.GetScreenshot()!
        ?? androidDriver?.GetScreenshot()!
        ?? appiumAndroidDriver?.GetScreenshot()!
        ?? appiumIosDriver?.GetScreenshot()!;

    }

    public AppiumWebElement FindElementsByAccessibilityId(string id)
    {
      if (driverType == "iOS")
      {
        return iosDriver?.FindElementByAccessibilityId(id)!
          ?? appiumIosDriver?.FindElementByAccessibilityId(id)!;
      }
      else
      {
        return androidDriver?.FindElementByAccessibilityId(id)!
          ?? appiumAndroidDriver?.FindElementByAccessibilityId(id)!;
      }
    }

    public AppiumWebElement FindElementByXPath(string xpath)
    {
      if (driverType == "iOS")
      {
        return iosDriver?.FindElementByXPath(xpath)!
         ?? appiumIosDriver?.FindElementByXPath(xpath)!;
      }
      else
      {
        return androidDriver?.FindElementByXPath(xpath)!
          ?? appiumAndroidDriver?.FindElementByXPath(xpath)!;
      }
    }
  }
}
