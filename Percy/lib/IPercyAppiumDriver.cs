using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace PercyIO.Appium
{
  internal interface IPercyAppiumDriver
  {
    String GetType();
    String Orientation();
    List<string> getElementIds(List<object> elements);
    IPercyAppiumCapabilities GetCapabilities();
    IDictionary<string, object> GetSessionDetails();
    String sessionId();
    String getSessionId();
    String ExecuteScript(String script);
    String GetHost();

    PercyAppiumElement FindElementsByAccessibilityId(String id);

    PercyAppiumElement FindElementByXPath(String xpath);
    String GetScreenshot();
  }
}
