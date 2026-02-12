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

    public Dictionary<string, object> GetCapabilities()
    {
      return capabilites;
    }

    public Dictionary<string, object> GetCapability(Object driver)
    {
      var capabilityObject = ReflectionUtils.PropertyCall<object>(driver, "Capabilities");
      var type = capabilityObject.GetType();
      
      FieldInfo fieldInfo = type.GetField("capabilities", BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
      if (fieldInfo == null)
      {
        fieldInfo = type.GetField("_capabilities", BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
      }
      
      if (fieldInfo == null)
      {
         // Fallback: check for 'caps'
         fieldInfo = type.GetField("caps", BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
      }

      if (fieldInfo != null)
      {
         var res = fieldInfo.GetValue(capabilityObject);
         if (res is Dictionary<string, object> dict)
         {
             return dict;
         }
      }

      // If still fails, try checking if property exposes dictionary
      // This is a last resort fallback
      Utils.Log("Could not reflectively find capabilities dictionary. Percy options might be missing.", "debug");
      return new Dictionary<string, object>();
    }
  }
}
