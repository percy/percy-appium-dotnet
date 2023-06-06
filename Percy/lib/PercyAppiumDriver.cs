using System;
using System.Collections.Generic;
using System.Reflection;

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
      return RefectionUtils.PropertyCall<String>(driver, "Orientation");
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

    public IDictionary<string, object> GetSessionDetails()
    {

      var key = "session_" + sessionId();
      if (AppPercy.cache.Get(key) == null)
      {
        var sess = RefectionUtils.PropertyCall<IDictionary<string, object>>(driver, "SessionDetails");
        AppPercy.cache.Store(key, sess);
      }
      return (IDictionary<string, object>)AppPercy.cache.Get(key);
    }

    public String sessionId()
    {
      return RefectionUtils.PropertyCall<String>(driver, "SessionId");
    }

    public String ExecuteScript(String script)
    {

      return ExecuteScript(driver, script)?.ToString()!;
    }

    public string GetHost()
    {
      return GetHostV5(driver)?.ToString()! ?? GetHostV4(driver)?.ToString()!;
    }

    public String GetScreenshot()
    {
      var screenshot = RefectionUtils.MethodCall<object>(driver, "GetScreenshot", null);
      return RefectionUtils.PropertyCall<String>(screenshot, "AsBase64EncodedString")!;
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
      var type = obj.GetType();
      var property = type.GetProperty("CommandExecutor", BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
      var commandExecutor = property?.GetValue(obj);
      var uri = commandExecutor?.GetType().GetField("URL", BindingFlags.Instance | BindingFlags.NonPublic);
      var remoteServerUri = commandExecutor?.GetType().GetField("remoteServerUri", BindingFlags.Instance | BindingFlags.NonPublic);
      var value = uri?.GetValue(commandExecutor) ?? remoteServerUri?.GetValue(commandExecutor);
      return value;
    }

    private Object? GetHostV5(Object obj)
    {
      var type = obj.GetType();
      PropertyInfo property = type.GetProperty("CommandExecutor", BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
      var commandExecutor = property?.GetValue(obj);
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
          AppPercy.Log($"Driver doesn't have method FindElement by: {by}", "debug");
        }
      }
      catch (Exception)
      {
        AppPercy.Log($"Got Error while running FindElement by: {by}", "debug");
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
