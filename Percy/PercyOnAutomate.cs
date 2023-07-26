using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace PercyIO.Appium
{
  public class PercyOnAutomate : IPercy
  {
    private IPercyAppiumDriver percyAppiumDriver;
    private Boolean isPercyEnabled;
    private static readonly string ignoreElementKey = "ignore_region_appium_elements";
    private static readonly string considerElementKey = "consider_region_appium_elements";

    public PercyOnAutomate(Object driver)
    {
      if(!Utils.isValidDriverObject(driver))
      {
        Utils.Log("Driver object is not the type of AndroidDriver or IOSDriver. The percy command may break.", "warn");
      }
      this.percyAppiumDriver = new PercyAppiumDriver(driver);
      this.isPercyEnabled = CliWrapper.Healthcheck();
    }

    public void Screenshot(String name, IEnumerable<KeyValuePair<string, object>>? options = null) 
    {
      if(!isPercyEnabled) return;
      try
      {
          Dictionary<string, object> userOptions = new Dictionary<string, object>();
          if(options != null) {
              userOptions = options.ToDictionary(kv => kv.Key, kv => kv.Value);

              if(userOptions.ContainsKey(ignoreElementKey)) {
                  List<object>? ignoreElements = userOptions[ignoreElementKey] as List<object>;
                  if(ignoreElements != null)
                  {
                      List<string> elementIds = percyAppiumDriver.GetElementIds(ignoreElements);
                      userOptions.Remove(ignoreElementKey);
                      userOptions["ignore_region_elements"] = elementIds;
                  }
              }

              if(userOptions.ContainsKey(considerElementKey)) {
                  List<object>? considerElements = userOptions[considerElementKey] as List<object>;
                  if(considerElements != null)
                  {
                      List<string> elementIds = percyAppiumDriver.GetElementIds(considerElements);
                      userOptions.Remove(considerElementKey);
                      userOptions["consider_region_elements"] = elementIds;
                  }
              }
          }

          CliWrapper.PostPOAScreenshot(name, percyAppiumDriver.getSessionId(), percyAppiumDriver.GetHost().TrimEnd('/'), percyAppiumDriver.GetCapabilities(), userOptions);
      }
      catch(Exception error)
      {
          Utils.Log($"Could not take Percy Screenshot \"{name}\"");
          Utils.Log(error.ToString(), "debug");
      }
    }
    public void Screenshot(String name, ScreenshotOptions? options, bool fullScreen) {
      throw new Exception("Options need to be passed using Dictionary for: " + name);
    }

    public class Options : Dictionary<string, object> {}
  }
}
