using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;

namespace PercyIO.Appium
{
  internal interface IPercyAppiumDriver
  {
    String GetType();
    String Orientation();
    ICapabilities GetCapabilities();
    System.Collections.Generic.IDictionary<string, object> GetSessionDetails();
    String sessionId();
    String ExecuteScript(String script);
    String GetHost();

    AppiumWebElement FindElementsByAccessibilityId(String id);

    AppiumWebElement FindElementByXPath(String xpath);
    Screenshot GetScreenshot();
  }
}
