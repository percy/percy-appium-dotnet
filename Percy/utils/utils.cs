using System;
using System.Collections.Generic;
using System.Reflection;

namespace PercyIO.Appium
{
  internal class Utils
  {
    public static readonly string[] MyConstants = { "OpenQA.Selenium.Appium.Android.AndroidDriver", "OpenQA.Selenium.Appium.iOS.IOSDriver" };
    public static Object ReflectionMethodHelper(Object obj, String methodName, params object[] args)
    {
      try
      {
        Type objectType = obj.GetType();
        MethodInfo method = objectType.GetMethod(methodName);
        if (method != null)
        {
          return method.Invoke(obj, args);
        }

        return null;
      }
      catch (Exception e)
      {
        return null;
      }
    }

    public static Object ExecuteScript(Object obj, String script)
    {
      Type objectType = obj.GetType();
      MethodInfo method = objectType.GetMethod("ExecuteScript", new Type[] { typeof(string), typeof(object[]) });
      if (method != null)
      {
        return method.Invoke(obj, new object[] { script, new object[] { null } });
      }
      return null;
    }

    public static Object FindElement(Object obj, String by, String value)
    {
      try
      {
        Type objectType = obj.GetType();
        MethodInfo method = objectType.GetMethod("FindElement", new Type[] { typeof(string), typeof(string) });
        if (method != null)
        {
          return method.Invoke(obj, new object[] { by, value });
        }
        else
        {
          AppPercy.Log($"Driver doesn't have method FindElement by: {by}", "debug");
        }
      }
      catch (Exception)
      {
        AppPercy.Log($"Got Error while running FindElement by: {by}", "debug");
      }

      return null;
    }

    public static Object? ReflectionPropertyHelper(Object obj, String propertyName)
    {
      Type objectType = obj.GetType();
      PropertyInfo property = objectType.GetProperty(propertyName);
      return property?.GetValue(obj);
    }

    public static Object? GetHostV5(Object obj)
    {
      try
      {
        var type = obj.GetType();
        PropertyInfo property = type.GetProperty("CommandExecutor", BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
        var commandExecutor = property?.GetValue(obj);
        var field = commandExecutor?.GetType().GetField("RealExecutor", BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
        var realExecutor = field?.GetValue(commandExecutor);
        var remoteServerUri = realExecutor?.GetType().GetField("remoteServerUri", BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
        return remoteServerUri?.GetValue(realExecutor);
      }
      catch (Exception e)
      {
        AppPercy.Log($"Got Error while running GetHostV5", "debug");
        return null;
      }
    }
    public static Dictionary<string, object> GetCapability(Object obj)
    {
      var type = obj.GetType();
      var fieldInfo = type.GetField("capabilities", BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
      return (Dictionary<string, object>)fieldInfo.GetValue(obj);
    }

    public static Object? GetHostV4(Object obj)
    {
        var type = obj.GetType();
        var property = type.GetProperty("CommandExecutor", BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
        var commandExecutor = property?.GetValue(obj);
        var uri = commandExecutor?.GetType().GetField("URL", BindingFlags.Instance | BindingFlags.NonPublic);
        var remoteServerUri = commandExecutor?.GetType().GetField("remoteServerUri", BindingFlags.Instance | BindingFlags.NonPublic);
        var value = uri?.GetValue(commandExecutor) ?? remoteServerUri?.GetValue(commandExecutor);
        return value;
    }

    public static Boolean isValidDriverObject(Object obj)
    {
      String type = obj.GetType().ToString();
      foreach (string constant in MyConstants)
      {
        if (type.Contains(constant))
        {
          return true;
        }
      }

      return false;
    }
  }
}
