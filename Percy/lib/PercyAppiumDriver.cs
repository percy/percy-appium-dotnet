using System;
using System.Reflection;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Appium.iOS;

namespace PercyIO.Appium
{
  internal class PercyAppiumDriver : IPercyAppiumDriver
  {
    private object driver;
    private String driverType;
    private IOSDriver<IOSElement> iosDriver;
    private AndroidDriver<AndroidElement> androidDriver;

    internal PercyAppiumDriver(AndroidDriver<AndroidElement> driver)
    {
      this.driver = driver;
      this.driverType = "Android";
      this.androidDriver = driver as AndroidDriver<AndroidElement>;
    }

    internal PercyAppiumDriver(IOSDriver<IOSElement> driver)
    {
      this.driver = driver;
      this.driverType = "iOS";
      this.iosDriver = driver as IOSDriver<IOSElement>;
    }

    public new String GetType()
    {
      return driverType;
    }

    public String Orientation()
    {
      if (driverType == "iOS")
      {
        return iosDriver.Orientation.ToString();
      }
      else
      {
        return androidDriver.Orientation.ToString();
      }
    }

    public ICapabilities GetCapabilities()
    {
      if (driverType == "iOS")
      {
        return iosDriver.Capabilities;
      }
      else
      {
        return androidDriver.Capabilities;
      }
    }

    public System.Collections.Generic.IDictionary<string, object> GetSessionDetails()
    {
      if (driverType == "iOS")
      {
        return iosDriver.SessionDetails;
      }
      else
      {
        return androidDriver.SessionDetails;
      }
    }

    public String sessionId()
    {
      if (driverType == "iOS")
      {
        return iosDriver.SessionId.ToString();
      }
      else
      {
        return androidDriver.SessionId.ToString();
      }
    }

    public String ExecuteScript(String script)
    {
      if (driverType == "iOS")
      {
        return iosDriver.ExecuteScript(script).ToString()!;
      }
      else
      {
        return androidDriver.ExecuteScript(script).ToString()!;
      }
    }

    public string GetHost()
    {
      Type type = driver.GetType();
      var property = type.GetProperty("CommandExecutor",System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
      var commandExecutor = property?.GetValue(driver);
      var uri = commandExecutor?.GetType().GetField("URL", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
      var value = uri?.GetValue(commandExecutor);
      return value?.ToString();
    }

    public Screenshot GetScreenshot()
    {
      if (driverType == "iOS")
      {
        return iosDriver.GetScreenshot();
      }
      else
      {
        return androidDriver.GetScreenshot();
      }
    }
  }
}
