using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Appium.iOS;
using System.Collections.Generic;
using OpenQA.Selenium.Appium;
using System.Reflection;

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
      var type = driver.GetType();
      var property = type.GetProperty("CommandExecutor", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
      var commandExecutor = property?.GetValue(driver);
      var uri = commandExecutor?.GetType().GetField("URL", BindingFlags.Instance | BindingFlags.NonPublic);
      var remoteServerUri = commandExecutor?.GetType().GetField("remoteServerUri", BindingFlags.Instance | BindingFlags.NonPublic);
      var value = uri?.GetValue(commandExecutor) ?? remoteServerUri?.GetValue(commandExecutor);
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
      return iosDriver?.FindElementByAccessibilityId(id)!
        ?? appiumIosDriver?.FindElementByAccessibilityId(id)!
        ?? androidDriver?.FindElementByAccessibilityId(id)!
        ?? appiumAndroidDriver?.FindElementByAccessibilityId(id)!;
    }

    public AppiumWebElement FindElementByXPath(string xpath)
    {
      return iosDriver?.FindElementByXPath(xpath)!
        ?? appiumIosDriver?.FindElementByXPath(xpath)!
        ?? androidDriver?.FindElementByXPath(xpath)!
        ?? appiumAndroidDriver?.FindElementByXPath(xpath)!;
    }
  }
}

// System.Uri remoteServerUri
// System.TimeSpan serverResponseTimeout
// Boolean enableKeepAlive
// Boolean isDisposed
// System.Net.IWebProxy proxy
// OpenQA.Selenium.Remote.CommandInfoRepository commandInfoRepository
// System.EventHandler`1[OpenQA.Selenium.Remote.SendingRemoteHttpRequestEventArgs] SendingRemoteHttpRequest