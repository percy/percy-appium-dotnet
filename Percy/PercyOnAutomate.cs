using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace PercyIO.Appium
{
  public class PercyOnAutomate : IPercy
  {
    public static readonly bool DEBUG = Environment.GetEnvironmentVariable("PERCY_LOGLEVEL") == "debug";
    private IPercyAppiumDriver percyAppiumDriver;
    private Boolean isPercyEnabled;
    private static readonly string ignoreElementKey = "ignore_region_appium_elements";

    public PercyOnAutomate(Object driver)
    {
      this.percyAppiumDriver = new PercyAppiumDriver(driver);
      this.isPercyEnabled = CliWrapper.Healthcheck();
    }

    public void Screenshot(String name, IEnumerable<KeyValuePair<string, object>>? options) 
    {
      if(!isPercyEnabled) return;
      try
      {
          Dictionary<string, object> userOptions = new Dictionary<string, object>();
          if(options != null) {
              userOptions = options.ToDictionary(kv => kv.Key, kv => kv.Value);

              if(userOptions.ContainsKey(ignoreElementKey)) {
                  JArray? ignoreElements = userOptions[ignoreElementKey] as JArray;

                  if(ignoreElements != null)
                  {
                      List<string> elementIds = percyAppiumDriver.getElementIds(ignoreElements);
                      userOptions.Remove(ignoreElementKey);
                      userOptions["ignore_region_elements"] = elementIds;
                  }
              }
          }

          CliWrapper.PostPOAScreenshot(name, percyAppiumDriver.getSessionId(), percyAppiumDriver.GetHost().TrimEnd('/'), percyAppiumDriver.GetCapabilities(), userOptions);
      }
      catch(Exception error)
      {
          Log($"Could not take Percy Screenshot \"{name}\"");
          Log(error.ToString(), "debug");
      }
    }
    public void Screenshot(String name, ScreenshotOptions? options, bool fullScreen) {
      throw new Exception("Options need to be passed using Dictionary for: " + name);
    }

    public class Options : Dictionary<string, object> {}

    internal static void Log(String message, String logLevel = "info")
    {
      if (logLevel == "debug" && DEBUG)
      {
        string label = "percy:dotnet";
        LogMessage(message, label, "91m");
      }
      else if (logLevel == "info")
      {
        string label = "percy";
        LogMessage(message, label);
      }
      else if (logLevel == "warn")
      {
        string label = "percy:dotnet";
        LogMessage(message, label, "93m");
      }
    }

    private static void LogMessage(String message, String label, String color = "39m")
    {
      Console.WriteLine($"[\u001b[35m{label}\u001b[{color}] {message}");
    }
  }
}
