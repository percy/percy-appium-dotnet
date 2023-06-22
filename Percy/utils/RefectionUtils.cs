using System;
using System.Collections.Generic;
using System.Reflection;

namespace PercyIO.Appium
{
  internal class ReflectionUtils
  {
    public static T MethodCall<T>(Object obj, String methodName, params object[] args)
    {
      try
      {
        Type objectType = obj.GetType();
        MethodInfo method = objectType.GetMethod(methodName);
        if (method == null) {
          throw new PercyException($"Method {methodName} not found for class {obj.GetType()}");
        }
        var methodObj = method.Invoke(obj, args);
        if(methodObj is T result)
        {
          return result;
        } else if (methodObj == null) {
          return default(T);
        } else {
          throw new PercyException($"Type does not match for method {methodName}");
        }
      }
      catch (Exception e)
      {
        throw new PercyException(e.ToString());
      }
    }

    public static T PropertyCall<T>(Object obj, String propertyName)
    {
      try
      {
        Type objectType = obj.GetType();
        PropertyInfo property = objectType.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
        if (property == null) {
          throw new PercyException($"Property {propertyName} not found for class {obj.GetType()}");
        }
        var propertyObj = property.GetValue(obj);
        if (propertyObj is T result)
        {
          return result;
        } else if (propertyObj == null) {
          return default(T);
        } else {
          throw new PercyException($"Type does not match for property {propertyName}");
        }
      }
      catch (Exception e)
      {
        throw new PercyException(e.ToString());
      }
    }
  }
}
