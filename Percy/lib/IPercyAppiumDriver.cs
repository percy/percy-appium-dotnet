using System;
using System.Collections.Generic;
namespace PercyIO.Appium
{
  internal interface IPercyAppiumDriver
  {
    String GetType();
    String Orientation();
    PercyAppiumCapabilities GetCapabilities();
    IDictionary<string, object> GetSessionDetails();
    String sessionId();
    String ExecuteScript(String script);
    String GetHost();

    PercyAppiumElement FindElementsByAccessibilityId(String id);

    PercyAppiumElement FindElementByXPath(String xpath);
    String GetScreenshot();
  }
}
