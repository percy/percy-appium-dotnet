using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json.Linq;

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
      var type = GetCapabilities().getValue<String>("platformName");
      return type ?? "";
    }

    public String Orientation()
    {
      return ReflectionUtils.PropertyCall<String>(driver, "Orientation");
    }

    public List<string> GetElementIds(List<object> elements) {
      List<string> ignoredElementsArray = new List<string>();
      for (int index = 0; index < elements.Count; index++)
      {
        var element = new PercyAppiumElement(elements[index]);
        ignoredElementsArray.Add(element.id);
      }
      return ignoredElementsArray;
    }

    public IPercyAppiumCapabilities GetCapabilities()
    {
      var key = "caps_" + sessionId();
      if (AppPercy.cache.Get(key) == null)
      {
        var percyAppiumCapabilites = new PercyAppiumCapabilities(driver);
        AppPercy.cache.Store(key, percyAppiumCapabilites);
      }
      return (PercyAppiumCapabilities)AppPercy.cache.Get(key);
    }

    public String sessionId()
    {
      return ReflectionUtils.PropertyCall<Object>(driver, "SessionId").ToString();
    }

    public String ExecuteScript(String script)
    {

      return ExecuteScript(driver, script)?.ToString()!;
    }

    public int DownscaledWidth()
    {
      var manage = ReflectionUtils.MethodCall<Object>(driver, "Manage");
      var window = ReflectionUtils.PropertyCall<Object>(manage, "Window");
      var size = ReflectionUtils.PropertyCall<Object>(window, "Size");
      return (int)ReflectionUtils.PropertyCall<Object>(size, "Width");
    }

    public object ExecuteDriverScript(String script)
    {
      return ExecuteScript(driver, script);
    }

    public string getSessionId()
    {
      string sessionId = this.sessionId();
      if (sessionId == null)
      {
        sessionId = ((dynamic) driver).SessionId.ToString();
      }
      return sessionId;
    }

    public string GetHost()
    {
      var host = GetHostV5(driver)?.ToString() ?? GetHostV4(driver)?.ToString();
      if (host == null)
      {
        Utils.Log("Unable to extract remote server URI from driver. This may be due to driver implementation changes.", "debug");
      }
      return host;
    }

    public String GetScreenshot()
    {
      var screenshot = ReflectionUtils.MethodCall<object>(driver, "GetScreenshot", null);
      return ReflectionUtils.PropertyCall<String>(screenshot, "AsBase64EncodedString")!;
    }

    public PercyAppiumElement FindElementsByAccessibilityId(string id)
    {
      var element = FindElement(driver, "id", id);
      if (element != null)
      {
        return new PercyAppiumElement(element);
      }
      return null;
    }

    public PercyAppiumElement FindElementByXPath(string xpath)
    {
      var element = FindElement(driver, "xpath", xpath);
      if (element != null)
      {
        return new PercyAppiumElement(element);
      }
      return null;
    }

    private Object? GetHostV4(Object obj)
    {
      try
      {
        var commandExecutor = ReflectionUtils.PropertyCall<Object>(obj, "CommandExecutor");
        if (commandExecutor == null) return null;

        var uri = commandExecutor?.GetType().GetField("URL", BindingFlags.Instance | BindingFlags.NonPublic);
        var remoteServerUri = commandExecutor?.GetType().GetField("remoteServerUri", BindingFlags.Instance | BindingFlags.NonPublic);
        var remoteServerAddress = commandExecutor?.GetType().GetField("remoteServerAddress", BindingFlags.Instance | BindingFlags.NonPublic); // Added backward compatibility
        var addressOfRemoteServer = commandExecutor?.GetType().GetField("addressOfRemoteServer", BindingFlags.Instance | BindingFlags.NonPublic); // For some variants

        var value = uri?.GetValue(commandExecutor) 
                    ?? remoteServerUri?.GetValue(commandExecutor)
                    ?? remoteServerAddress?.GetValue(commandExecutor)
                    ?? addressOfRemoteServer?.GetValue(commandExecutor);
        return value;
      }
      catch (Exception e)
      {
         Utils.Log($"Failed to get host V4: {e.Message}", "debug");
         return null;
      }
    }

    private Object? GetHostV5(Object obj)
    {
      try
      {
        var commandExecutor = ReflectionUtils.PropertyCall<Object>(obj, "CommandExecutor");
        if (commandExecutor == null) return null;

        var field = commandExecutor?.GetType().GetField("RealExecutor", BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
        var realExecutor = field?.GetValue(commandExecutor);
        if (realExecutor == null) return null;

        var remoteServerUri = realExecutor?.GetType().GetField("remoteServerUri", BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
        var remoteServerAddress = realExecutor?.GetType().GetField("remoteServerAddress", BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic); // Appium v5 compatibility
        var addressOfRemoteServer = realExecutor?.GetType().GetField("addressOfRemoteServer", BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic); 

        return remoteServerUri?.GetValue(realExecutor) 
               ?? remoteServerAddress?.GetValue(realExecutor)
               ?? addressOfRemoteServer?.GetValue(realExecutor);
      }
      catch (Exception e)
      {
        Utils.Log($"Failed to get host V5: {e.Message}", "debug");
        return null;
      }
    }

    // FindElement method is overloaded so creating separate method
    private Object FindElement(Object obj, String by, String value)
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
          Utils.Log($"Driver doesn't have method FindElement by: {by}", "debug");
        }
      }
      catch (Exception)
      {
        Utils.Log($"Got Error while running FindElement by: {by}", "debug");
      }

      return null;
    }

    // ExecuteScript is overloaded so creating separate method
    private Object ExecuteScript(Object obj, String script)
    {
      Type objectType = obj.GetType();
      MethodInfo method = objectType.GetMethod("ExecuteScript", new Type[] { typeof(string), typeof(object[]) });
      if (method != null)
      {
        return method.Invoke(obj, new object[] { script, new object[] { null } });
      }
      return null;
    }
  }
}
