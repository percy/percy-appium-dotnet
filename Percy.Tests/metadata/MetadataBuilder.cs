using System;
using System.Collections.Generic;
using Moq;
using PercyIO.Appium;

namespace Percy.Tests
{
  class MetadataBuilder
  {
    public static Mock<IPercyAppiumDriver> mockDriver(String os, dynamic? percyOptions = null)
    {
      Mock<IPercyAppiumDriver> percyAppiumDriver = new Mock<IPercyAppiumDriver>();
      var caps = new PercyAppiumCapabilities();
      caps.SetCapability(CapabilityBuilder(os, percyOptions));
      percyAppiumDriver.Setup(x => x.GetCapabilities())
        .Returns(caps);
      percyAppiumDriver.Setup(x => x.sessionId())
        .Returns("session-1");
      percyAppiumDriver.Setup(x => x.GetType())
        .Returns(os);
      percyAppiumDriver.Setup(x => x.GetScreenshot())
        .Returns("c2hvcnRlc3Q=");
      return percyAppiumDriver;
    }
    public static Dictionary<String, Object> CapabilityBuilder(String os, dynamic? percyOption = null)
    {
      var capabilites = new Dictionary<String, Object>();
      var deviceName = "Samsung_gs22u";
      var platform = os;
      if (os == "iOS")
      {
        deviceName = "iPhone_11";
        platform = "iOS";
        capabilites.Add("deviceName", deviceName);
      }
      else
      {
        capabilites.Add("device", deviceName);
      }
      var desired = new Dictionary<string, object>(){
        {"deviceName", deviceName}
      };
      var viewportRect = new Dictionary<string, object>(){
        {"top", 100L},
        {"height", 1000L}
      };
      if (percyOption != null)
      {
        if (percyOption.jwp)
        {
          capabilites.Add("percy.ignoreErrors", percyOption.ignoreError);
          capabilites.Add("percy.enabled", percyOption.percyEnabled);
        }
        else
        {
          capabilites.Add("percyOptions", new Dictionary<string, object>(){
            {"enabled", percyOption.percyEnabled},
            {"ignoreErrors", percyOption.ignoreError},
          });
        }
      }
      capabilites.Add("deviceScreenSize", "1280x1420");
      capabilites.Add("platformName", platform);
      capabilites.Add("viewportRect", viewportRect);
      capabilites.Add("desired", desired);
      capabilites.Add("platformVersion", "12.0");
      return capabilites;
    }

    public static Dictionary<String, Object> DesiredCaps()
    {
      var capabilites = new Dictionary<String, Object>();
      return capabilites;
    }
  }
}