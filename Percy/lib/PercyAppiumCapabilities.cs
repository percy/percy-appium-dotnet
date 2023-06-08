using System;
using System.Collections.Generic;
using System.Reflection;

namespace PercyIO.Appium
{
  internal class PercyAppiumCapabilities : IPercyAppiumCapabilities
  {
    private Dictionary<string, object> capabilites;
    internal PercyAppiumCapabilities()
    {

    }
    internal PercyAppiumCapabilities(Object driver)
    {
      SetCapability(GetCapability(driver));
    }

    public T getValue<T>(String key)
    {
      if(capabilites.ContainsKey(key) && capabilites[key] is T result)
      {
        return result;
      }
      return default(T);
    }

    public void SetCapability(Dictionary<string, object> capabilites)
    {
      this.capabilites = capabilites;
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
