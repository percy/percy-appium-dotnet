using System;
using System.Collections.Generic;

namespace PercyIO.Appium
{
  internal class PercyOptions
  {
    private IPercyAppiumDriver percyAppiumDriver;
    private String sessionId;

    internal PercyOptions(IPercyAppiumDriver percyAppiumDriver)
    {
      this.percyAppiumDriver = percyAppiumDriver;
      this.sessionId = percyAppiumDriver.sessionId();
    }

    internal bool PercyEnabled()
    {
      var percyOptionsW3CProtocol = getPercyOptions();
      var percyEnabledJsonProtocol = Utils.ReflectionMethodHelper(percyAppiumDriver.GetCapabilities(), "GetCapability", "percy.enabled");
      if (percyOptionsW3CProtocol == null && percyEnabledJsonProtocol == null)
      {
        AppPercy.Log("Percy options not provided in capabilitiies, considering enabled", "debug");
        return true;
      }
      else if ((percyEnabledJsonProtocol?.ToString() == "False") ||
              (percyOptionsW3CProtocol?["enabled"]?.ToString() == "False"))
      {
        AppPercy.Log("App Percy is disabled in capabilities");
        return false;
      }
      return true;
    }

    internal void SetPercyIgnoreErrors()
    {
      var percyOptionsW3CProtocol = getPercyOptions();
      var percyIgnoreErrorsJsonProtocol = Utils.ReflectionMethodHelper(percyAppiumDriver.GetCapabilities(), "GetCapability", "percy.ignoreErrors");
      if (percyOptionsW3CProtocol == null && percyIgnoreErrorsJsonProtocol == null)
      {
        AppPercy.Log("Percy options not provided in capabilitiies, ignoring errors by default", "debug");
        return;
      }
      else if ((percyIgnoreErrorsJsonProtocol?.ToString() == "False")
              || (percyOptionsW3CProtocol?["ignoreErrors"]?.ToString() == "False"))
      {
        AppPercy.ignoreErrors = false;
      }
      return;
    }

    internal Dictionary<string, object> getPercyOptions()
    {
      if (AppPercy.cache.Get("percyOptions_" + sessionId) == null)
      {
        var options = Utils.ReflectionMethodHelper(percyAppiumDriver.GetCapabilities(), "GetCapability", "percyOptions");
        AppPercy.cache.Store("percyOptions_" + sessionId, options);
      }
      return (Dictionary<string, object>)AppPercy.cache.Get("percyOptions_" + sessionId);
    }
  }
}
