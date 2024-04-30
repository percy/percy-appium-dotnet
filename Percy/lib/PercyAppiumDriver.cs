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
      return GetCapabilities().getValue<String>("platformName")!;
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
      return GetHostV5(driver)?.ToString()! ?? GetHostV4(driver)?.ToString()!;
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
      var commandExecutor = ReflectionUtils.PropertyCall<Object>(obj, "CommandExecutor");
      var uri = commandExecutor?.GetType().GetField("URL", BindingFlags.Instance | BindingFlags.NonPublic);
      var remoteServerUri = commandExecutor?.GetType().GetField("remoteServerUri", BindingFlags.Instance | BindingFlags.NonPublic);
      var value = uri?.GetValue(commandExecutor) ?? remoteServerUri?.GetValue(commandExecutor);
      return value;
    }

    private Object? GetHostV5(Object obj)
    {
      var commandExecutor = ReflectionUtils.PropertyCall<Object>(obj, "CommandExecutor");
      var field = commandExecutor?.GetType().GetField("RealExecutor", BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
      var realExecutor = field?.GetValue(commandExecutor);
      var remoteServerUri = realExecutor?.GetType().GetField("remoteServerUri", BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
      return remoteServerUri?.GetValue(realExecutor);
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
