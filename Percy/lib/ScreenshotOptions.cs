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
    public int TopScrollviewOffset { get; set; } = 0;
    public int BottomScrollviewOffset { get; set; } = 0;
    public String? Orientation { get; set; }
    public bool FullScreen { get; set; } = false;
    public bool FullPage { get; set; } = false;
    public int? ScreenLengths { get; set; }
    public Boolean? Sync { get; set; }
    public String? TestCase { get; set; }
    public String? Labels { get; set; }
    public String? ThTestCaseExecutionId { get; set; }
    public List<String> IgnoreRegionXpaths { get; set; } = new List<string>();
    public List<String> IgnoreRegionAccessibilityIds { get; set; } = new List<string>();
    public List<Object> IgnoreRegionAppiumElements { get; set; } = new List<Object>();
    public List<Region> CustomIgnoreRegions { get; set; } = new List<Region>();
    public List<String> ConsiderRegionXpaths { get; set; } = new List<string>();
    public List<String> ConsiderRegionAccessibilityIds { get; set; } = new List<string>();
    public List<Object> ConsiderRegionAppiumElements { get; set; } = new List<Object>();
    public List<Region> CustomConsiderRegions { get; set; } = new List<Region>();
    public String? ScrollableXpath { get; set; } = null;
    public String? ScrollableId { get; set; } = null;
  }
}
