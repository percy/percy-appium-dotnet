// Include namespace system
using System;
using System.Collections.Generic;
using OpenQA.Selenium.Appium;
using Newtonsoft.Json.Linq;

namespace PercyIO.Appium
{

  public class ScreenshotOptions
  {
    public String? DeviceName { get; set; }
    public int StatusBarHeight { get; set; } = -1;
    public int NavBarHeight { get; set; } = -1;
    public String? Orientation { get; set; }
    public bool FullScreen { get; set; } = false;
    public bool FullPage { get; set; } = false;
    public int? ScreenLengths { get; set; }
    public List<String> Xpaths { get; set; } = new List<string>();
    public List<String> AccessibilityIds { get; set; } = new List<string>();
    public List<AppiumWebElement> AppiumElements { get; set; } = new List<AppiumWebElement>();
    public List<JObject> CustomIgnoreRegions { get; set; } = new List<JObject>();
    public String? ScrollableXpath { get; set; } = null;
    public String? ScrollableId { get; set; } = null;
  }
}
