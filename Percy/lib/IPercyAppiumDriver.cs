using System;
using System.Collections.Generic;
namespace PercyIO.Appium
{
  internal interface IPercyAppiumDriver
  {
    String GetType();
    String Orientation();
    Object GetCapabilities();
    IDictionary<string, object> GetSessionDetails();
    String sessionId();
    String ExecuteScript(String script);
    String GetHost();

    Object FindElementsByAccessibilityId(String id);

    Object FindElementByXPath(String xpath);
    Object GetScreenshot();
  }
}
