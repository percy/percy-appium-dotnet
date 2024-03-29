using System;
using System.IO;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace PercyIO.Appium
{
  internal class MetadataHelper
  {
    internal static Func<IPercyAppiumDriver, String, int, int, String, String, Metadata> Resolve =
    (driver, deviceName, statusBar, navBar, orientation, platformVersion) =>
    {
      var driverClass = "";
      try
      {
        driverClass = driver.GetType();
        if (driverClass.Contains("Android"))
        {
          return new AndroidMetadata(driver, deviceName, statusBar, navBar, orientation, platformVersion);
        }
        else if (driverClass.Contains("iOS"))
        {
          return new IosMetadata(driver, deviceName, statusBar, navBar, orientation, platformVersion);
        }
        else
        {
          throw new Exception("Driver class not found");
        }
      }
      catch (Exception)
      {
        Utils.Log("Unsupported driver class, " + driverClass);
      }
      return null;
    };

    internal static Func<String, String, int> ValueFromStaticDevicesInfo = (key, deviceName) =>
    {
      var JsonObject = (JObject)GetDevicesJson().GetValue(deviceName);
      if (JsonObject == null)
      {
        return 0;
      }
      return (int)JsonObject.GetValue(key);
    };

    private static JObject GetDevicesJson()
    {
      if (AppPercy.cache.Get("getDevicesJson") == null)
      {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceStream = assembly.GetManifestResourceStream("Percy.resources.devices.json");
        var data = JObject.Parse(new StreamReader(resourceStream).ReadToEnd());
        AppPercy.cache.Store("getDevicesJson", data);
      }
      return (JObject)AppPercy.cache.Get("getDevicesJson");
    }
  }
}
