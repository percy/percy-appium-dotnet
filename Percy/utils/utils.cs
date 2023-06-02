using System;
using System.Linq;
using System.Reflection;

namespace PercyIO.Appium
{
  internal class Utils
  {
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

    public static Object FindElement(Object obj, String by ,String value)
    {
      Type objectType = obj.GetType();
      MethodInfo method = objectType.GetMethod("FindElement", new Type[] { typeof(string), typeof(string) });
      if (method != null)
      {
        return method.Invoke(obj, new object[] { by, value});
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
        return null;
      }
    }

    public static Object? GetHostV4(Object obj)
    {
      try
      {
        var type = obj.GetType();
        var property = type.GetProperty("CommandExecutor", BindingFlags.Instance | BindingFlags.NonPublic);
        var commandExecutor = property?.GetValue(obj);
        var uri = commandExecutor?.GetType().GetField("URL", BindingFlags.Instance | BindingFlags.NonPublic);
        var remoteServerUri = commandExecutor?.GetType().GetField("remoteServerUri", BindingFlags.Instance | BindingFlags.NonPublic);
        var value = uri?.GetValue(commandExecutor) ?? remoteServerUri?.GetValue(commandExecutor);
        return value;
      }
      catch (Exception e)
      {
        return null;
      }
    }
  }
}
