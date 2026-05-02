using System;
using System.Collections.Generic;
using System.Reflection;

namespace PercyIO.Appium
{
  internal class PercyAppiumCapabilities : IPercyAppiumCapabilities
  {
    private Dictionary<string, object> capabilites;

    // W3C WebDriver standard capabilities that never need vendor prefix
    private static readonly HashSet<string> W3CStandardCaps = new HashSet<string>
    {
      "browserName", "browserVersion", "platformName",
      "acceptInsecureCerts", "pageLoadStrategy", "proxy",
      "timeouts", "unhandledPromptBehavior"
    };

    internal PercyAppiumCapabilities()
    {

    }
    internal PercyAppiumCapabilities(Object driver)
    {
      SetCapability(GetCapability(driver));
    }

    public T getValue<T>(String key)
    {
      if (capabilites == null) return default(T);

      // W3C standard caps only use bare key
      if (W3CStandardCaps.Contains(key))
      {
        if (capabilites.ContainsKey(key) && capabilites[key] is T result)
          return result;
        return default(T);
      }

      // Try bare key first (Appium 1.x or already-resolved)
      if (capabilites.ContainsKey(key) && capabilites[key] is T bareResult)
        return bareResult;

      // Then try appium: prefixed key (Appium 2.x W3C protocol)
      string prefixedKey = "appium:" + key;
      if (capabilites.ContainsKey(prefixedKey) && capabilites[prefixedKey] is T prefixedResult)
        return prefixedResult;

      return default(T);
    }

    public void SetCapability(Dictionary<string, object> capabilites)
    {
      this.capabilites = capabilites;
    }

    public Dictionary<string, object> GetCapabilities()
    {
      return capabilites;
    }

    public Dictionary<string, object> GetCapability(Object driver)
    {
      var capabilityObject = ReflectionUtils.PropertyCall<object>(driver, "Capabilities");
      var type = capabilityObject.GetType();
      var fieldInfo = type.GetField("capabilities", BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
      return (Dictionary<string, object>)fieldInfo.GetValue(capabilityObject);
    }
  }
}
