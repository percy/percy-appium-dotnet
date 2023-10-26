using System;
using System.Collections.Generic;

namespace PercyIO.Appium
{
  // This acts as a factory
  internal class Percy
  {
    private IPercy percyClass;
    private Boolean isPercyEnabled;
    private static string poaSessionType = "automate";

    public Percy(Object driver) {
      if(!Utils.isValidDriverObject(driver))
      {
        Utils.Log("Driver object is not the type of AndroidDriver or IOSDriver. The percy command may break.", "warn");
      }
      isPercyEnabled = CliWrapper.Healthcheck();
      if(!isPercyEnabled)
      {
        return;
      }
      if(Env.GetSessionType() == poaSessionType) {
        percyClass = new PercyOnAutomate(driver);
      } else {
        percyClass = new AppPercy(driver);
      }
    }

    public void Screenshot(String name, Object options = null, Boolean fullScreen = false)
    {
      if(!isPercyEnabled)
      {
        return;
      }
      if(Env.GetSessionType() == poaSessionType) {
        percyClass.Screenshot(name, (IEnumerable<KeyValuePair<string, object>>) options);
      } else {
        percyClass.Screenshot(name, (ScreenshotOptions) options, fullScreen);
      }
    }
  }
}
