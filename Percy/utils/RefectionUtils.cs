using System;
using System.Collections.Generic;
using System.Reflection;

namespace PercyIO.Appium
{
  internal class RefectionUtils
  {
    public static T MethodCall<T>(Object obj, String methodName, params object[] args)
    {
      try
      {
        Type objectType = obj.GetType();
        MethodInfo method = objectType.GetMethod(methodName);
        var methodObj = method?.Invoke(obj, args);
        if(methodObj is T result)
        {
          return result;
        }
        return default(T);
      }
      catch (Exception e)
      {
        throw new CustomException($"Method {methodName} not found for class {obj.GetType()}", e);
      }
    }

    public static T PropertyCall<T>(Object obj, String propertyName)
    {
      try
      {
        Type objectType = obj.GetType();
        PropertyInfo property = objectType.GetProperty(propertyName);
        var propertyObj = property?.GetValue(obj);
        if (propertyObj is T result)
        {
          return result;
        }
        return default(T);
      }
      catch (Exception e)
      {
        throw new CustomException($"Method {propertyName} not found for class {obj.GetType()}", e);
      }
    }
  }
}
