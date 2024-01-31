using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace PercyIO.Appium
{
  public class AppPercy : IPercy
  {
    public static readonly bool DEBUG = Environment.GetEnvironmentVariable("PERCY_LOGLEVEL") == "debug";
    private Boolean isPercyEnabled;
    public static Boolean ignoreErrors = true;
    private PercyOptions percyOptions;
    private IPercyAppiumDriver percyAppiumDriver;
    private String sessionId;
    public static Cache<string, object> cache = new Cache<string, object>();

    public AppPercy(Object driver)
    {
      if(!Utils.isValidDriverObject(driver))
      {
        Utils.Log("Driver object is not the type of AndroidDriver or IOSDriver. The percy command may break.", "warn");
      }
      this.percyAppiumDriver = new PercyAppiumDriver(driver);
      setValues(this.percyAppiumDriver);
    }

    internal void setValues(IPercyAppiumDriver percyAppiumDriver)
    {
      this.percyOptions = new PercyOptions(percyAppiumDriver);
      this.isPercyEnabled = CliWrapper.Healthcheck();
      this.sessionId = percyAppiumDriver.sessionId();
    }

    public JObject? Screenshot(String name, ScreenshotOptions? options = null, Boolean fullScreen = false)
    {
      if (options == null)
      {
        options = new ScreenshotOptions();
      }
      options.FullScreen = fullScreen;
      if (!isPercyEnabled || !percyOptions.PercyEnabled())
      {
        return null;
      }
      percyOptions.SetPercyIgnoreErrors();
      try
      {
        GenericProvider provider;
        provider = ProviderResolver.ResolveProvider(percyAppiumDriver);
        JObject data =  provider.Screenshot(
          name,
          options
        );

        if (data?.TryGetValue("data", out JToken results) == true) {
          return (JObject)results;
        }
        return null;
      }
      catch (Exception e)
      {
        CliWrapper.PostFailedEvent(e.Message);
        if (e is PercyException)
        {
          Utils.Log("The method is not valid for current driver. Please contact us.", "warn");
        }
        Utils.Log("Error taking screenshot " + name);
        if (!ignoreErrors)
        {
          throw new Exception("Error taking screenshot " + name, e);
        }
        return null;
      }
    }

    public JObject? Screenshot(String name, IEnumerable<KeyValuePair<string, object>>? options) {
      if (options == null) {
        return Screenshot(name, null, false);
      }
      throw new Exception("Options need to be passed using Sceenshot Options for: " + name);
    }

    ~AppPercy()
    {
      AppPercy.cache.Remove("percyOptions_" + sessionId);
      AppPercy.cache.Remove("viewportRect_" + sessionId);
      AppPercy.cache.Remove("session_" + sessionId);
      AppPercy.cache.Remove("caps_" + sessionId);
    }
  }
}
