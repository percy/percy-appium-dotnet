using System;
using System.Collections.Generic;

namespace PercyIO.Appium
{
  internal class PercyAppiumDriver : IPercyAppiumDriver
  {
    private object driver;

    internal PercyAppiumDriver(Object driver)
    {
      this.driver = driver;
    }

    public new String GetType()
    {
      return GetCapabilities().getValue("platformName")?.ToString()!;
    }

    public String Orientation()
    {
      return Utils.ReflectionPropertyHelper(driver, "Orientation")?.ToString()!;
    }

    public PercyAppiumCapabilites GetCapabilities()
    {
      var key = "caps_" + sessionId();
      if (AppPercy.cache.Get(key) == null)
      {
        var capabilities = Utils.ReflectionPropertyHelper(driver, "Capabilities");
        var caps = Utils.GetCapability(capabilities);
        var percyAppiumCapabilites = new PercyAppiumCapabilites(caps);
        AppPercy.cache.Store(key, percyAppiumCapabilites);
      }
      return (PercyAppiumCapabilites)AppPercy.cache.Get(key);
    }

    public IDictionary<string, object> GetSessionDetails()
    {

      var key = "session_" + sessionId();
      if (AppPercy.cache.Get(key) == null)
      {
        var sess = Utils.ReflectionPropertyHelper(driver, "SessionDetails");
        AppPercy.cache.Store(key, sess);
      }
      return (IDictionary<string, object>)AppPercy.cache.Get(key);
    }

    public String sessionId()
    {
      return Utils.ReflectionPropertyHelper(driver, "SessionId")?.ToString()!;
    }

    public String ExecuteScript(String script)
    {

      return Utils.ExecuteScript(driver, script)?.ToString()!;
    }

    public string GetHost()
    {
      return Utils.GetHostV5(driver)?.ToString()! ?? Utils.GetHostV4(driver)?.ToString()!;
    }

    public String GetScreenshot()
    {
      var screenshot = Utils.ReflectionMethodHelper(driver, "GetScreenshot", null);
      return Utils.ReflectionPropertyHelper(screenshot, "AsBase64EncodedString")?.ToString()!;
    }

    public PercyAppiumElement FindElementsByAccessibilityId(string id)
    {
      var element = Utils.FindElement(driver, "id", id);
      if (element != null)
      {
        return new PercyAppiumElement(element);
      }
      return null;
    }

    public PercyAppiumElement FindElementByXPath(string xpath)
    {
      var element = Utils.FindElement(driver, "xpath", xpath);
      if (element != null)
      {
        return new PercyAppiumElement(element);
      }
      return null;
    }
  }
}
