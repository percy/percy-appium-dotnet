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
      return Utils.ReflectionMethodHelper(GetCapabilities(), "GetCapability", "platformName")?.ToString()!;
    }

    public String Orientation()
    {
      return Utils.ReflectionPropertyHelper(driver, "Orientation")?.ToString()!;
    }

    public Object GetCapabilities()
    {
      var key = "caps_" + sessionId();
      if (AppPercy.cache.Get(key) == null)
      {
        var caps = Utils.ReflectionPropertyHelper(driver, "Capabilities");
        AppPercy.cache.Store(key, caps);
      }
      return AppPercy.cache.Get(key);
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

    public Object GetScreenshot()
    {
      return Utils.ReflectionMethodHelper(driver, "GetScreenshot", null);
    }

    public Object FindElementsByAccessibilityId(string id)
    {
      return Utils.FindElement(driver, "id", id);
    }

    public Object FindElementByXPath(string xpath)
    {
      return Utils.FindElement(driver, "xpath", xpath);
    }
  }
}
