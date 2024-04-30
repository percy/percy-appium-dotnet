using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace PercyIO.Appium
{
  internal interface IPercyAppiumDriver
  {
    String GetType();
    String Orientation();
    List<string> GetElementIds(List<object> elements);
    IPercyAppiumCapabilities GetCapabilities();
    String sessionId();
    String getSessionId();
    String ExecuteScript(String script);
    String GetHost();

    PercyAppiumElement FindElementsByAccessibilityId(String id);

    PercyAppiumElement FindElementByXPath(String xpath);
    String GetScreenshot();
    object Execute(String script);
    int DownscaledWidth();
  }
}
