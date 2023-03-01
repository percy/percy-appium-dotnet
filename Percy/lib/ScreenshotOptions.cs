// Include namespace system
using System;

namespace PercyIO.Appium
{

  public class ScreenshotOptions
  {
    public String? DeviceName {get; set;}
    public int StatusBarHeight {get; set;} = -1;
    public int NavBarHeight {get; set;} = -1;
    public String? Orientation {get; set;}
    public bool FullScreen {get; set;} = false;
    public bool FullPage {get; set;} = false;
    public int? ScreenLengths {get; set;}
  }
}
