// Include namespace system
using System;
using System.Collections.Generic;

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
    public List<String> IgnoreRegionXpaths { get; set; } = new List<string>();
    public List<String> IgnoreRegionAccessibilityIds { get; set; } = new List<string>();
    public List<Object> IgnoreRegionAppiumElements { get; set; } = new List<Object>();
    public List<IgnoreRegion> CustomIgnoreRegions { get; set; } = new List<IgnoreRegion>();
    public String? ScrollableXpath { get; set; } = null;
    public String? ScrollableId { get; set; } = null;
    public bool ForceFullPage { get; set; } = false;
  }
}
