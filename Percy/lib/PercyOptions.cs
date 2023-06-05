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
      var percyEnabledJsonProtocol = percyAppiumDriver.GetCapabilities().getValue<String>("percy.enabled");
      if (percyOptionsW3CProtocol == null && percyEnabledJsonProtocol == null)
      {
        AppPercy.Log("Percy options not provided in capabilitiies, considering enabled", "debug");
        return true;
      }
      else if (percyEnabledJsonProtocol.IsFalse() || percyOptionsW3CProtocol["enabled"].IsFalse())
      {
        AppPercy.Log("App Percy is disabled in capabilities");
        return false;
      }
      return true;
    }

    internal void SetPercyIgnoreErrors()
    {
      var percyOptionsW3CProtocol = getPercyOptions();
      var percyIgnoreErrorsJsonProtocol = percyAppiumDriver.GetCapabilities().getValue<String>("percy.ignoreErrors");
      if (percyOptionsW3CProtocol == null && percyIgnoreErrorsJsonProtocol == null)
      {
        AppPercy.Log("Percy options not provided in capabilitiies, ignoring errors by default", "debug");
        return;
      }
      else if (percyIgnoreErrorsJsonProtocol.IsFalse() || percyOptionsW3CProtocol["ignoreErrors"].IsFalse())
      {
        AppPercy.ignoreErrors = false;
      }
      return;
    }

    internal Dictionary<string, object> getPercyOptions()
    {
      if (AppPercy.cache.Get("percyOptions_" + sessionId) == null)
      {
        var options = percyAppiumDriver.GetCapabilities().getValue<Dictionary<string, object>>("percyOptions");
        AppPercy.cache.Store("percyOptions_" + sessionId, options);
      }
      return (Dictionary<string, object>)AppPercy.cache.Get("percyOptions_" + sessionId);
    }
  }
}
