using System;
using System.Collections.Generic;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium;
using Moq;
using Xunit;
using PercyIO.Appium;
using Newtonsoft.Json.Linq;
using RichardSzalay.MockHttp;
using System.Net.Http;

namespace Percy.Tests
{
  public class AppPercyTest
  {

    // [Fact]
    // public void TestName_WithAndroid()
    // {
    //   AppPercy.cache.Clear();
    //   // Arrange
    //   var _androidPercyAppiumDriver = MetadataBuilder.mockDriver("Android");
    //   var info = new Dictionary<string, object>(){
    //       {"top", 100L},
    //       {"height", 1000L},
    //       {"width", 400L},
    //   };
    //   string url = "http://hub-cloud.browserstack.com/wd/hub";
    //   _androidPercyAppiumDriver.Setup(x => x.GetHost())
    //     .Returns(url);
    //   _androidPercyAppiumDriver.Setup(x => x.GetType())
    //     .Returns("Android");

    //   var arguments = new JObject();
    //   var response = JObject.FromObject(new
    //   {
    //     success = true,
    //     deviceName = "Samsung Galaxy S22",
    //     osVersion = "13.0",
    //     buildHash = "dummy_build_hash",
    //     sessionHash = "dummy_session_hash"
    //   });
    //   _androidPercyAppiumDriver.Setup(x => x.sessionId())
    //     .Returns(new SessionId("abc").ToString());
    //   _androidPercyAppiumDriver.Setup(x => x.GetType())
    //     .Returns("Android");
    //   _androidPercyAppiumDriver.Setup(x => x.ExecuteScript(It.IsAny<string>()))
    //     .Returns(response.ToString());
    //   var screenshot = new Screenshot("c2hvcnRlc3Q=");
    //    CliWrapper.Healthcheck = () =>
    //   {
    //     return true;
    //   };
    //   Mock<IPercyAppiumCapabilities> percyCapabilities = new Mock<IPercyAppiumCapabilities>();
    //   percyCapabilities.Setup(x => x.GetCapability(It.IsAny<Object>()))
    //     .Returns(MetadataBuilder.CapabilityBuilder("Android"));
    //   _androidPercyAppiumDriver.Setup(x => x.GetCapabilities())
    //     .Returns(percyCapabilities.Object);
    //   AppPercy appPercy = new AppPercy(_androidPercyAppiumDriver.Object);

    //   // Act
    //   appPercy.Screenshot("abc", null);
    //   // Assert
    //   //_androidPercyAppiumDriver.Verify(x => x.ExecuteScript(It.IsAny<string>()), Times.Exactly(2));
    // }

    // [Fact]
    // public void TestName_WithiOS()
    // {
    //   AppPercy.cache.Clear();
    //   // Arrange
    //   Mock<IPercyAppiumDriver> _iOSPercyAppiumDriver = new Mock<IPercyAppiumDriver>();
    //   Mock<ICapabilities> capabilities = new Mock<ICapabilities>();
    //   capabilities.Setup(x => x.GetCapability("platformName"))
    //      .Returns("ios");
    //   capabilities.Setup(x => x.GetCapability("deviceScreenSize"))
    //     .Returns("1280x1420");
    //   string url = "http://hub-cloud.browserstack.com/wd/hub";
    //   _iOSPercyAppiumDriver.Setup(x => x.GetHost())
    //     .Returns(url);
    //   _iOSPercyAppiumDriver.Setup(x => x.GetType())
    //     .Returns("iOS");
    //   // _iOSPercyAppiumDriver.Setup(x => x.GetCapabilities())
    //   //   .Returns(capabilities.Object);
    //   var info = new Dictionary<string, object>(){
    //     { "viewportRect", new Dictionary<string, object> {
    //       {"top", 100l},
    //       {"height", 1000l},
    //       {"width", 400l},
    //   }}};
    //   var screenshot = new Screenshot("c2hvcnRlc3Q=");
    //   // _iOSPercyAppiumDriver.Setup(x => x.GetScreenshot())
    //   //   .Returns(screenshot);
    //   _iOSPercyAppiumDriver.Setup(x => x.GetSessionDetails())
    //     .Returns(info);
    //   var arguments = new JObject();
    //   var response = JObject.FromObject(new
    //   {
    //     success = true,
    //     deviceName = "iPhone 13",
    //     osVersion = "15.0",
    //     buildHash = "dummy_build_hash",
    //     sessionHash = "dummy_session_hash"
    //   });
    //   _iOSPercyAppiumDriver.Setup(x => x.sessionId())
    //     .Returns(new SessionId("abc").ToString());
    //   _iOSPercyAppiumDriver.Setup(x => x.GetType())
    //     .Returns("iOS");
    //   _iOSPercyAppiumDriver.Setup(x => x.ExecuteScript(It.IsAny<string>()))
    //     .Returns(response.ToString());
    //   CliWrapper.Healthcheck = () =>
    //   {
    //     return true;
    //   };

    //   AppPercy appPercy = new AppPercy(_iOSPercyAppiumDriver.Object);

    //   // Act
    //   appPercy.Screenshot("abc", null);
    //   // Assert
    //  // _iOSPercyAppiumDriver.Verify(x => x.ExecuteScript(It.IsAny<string>()), Times.Exactly(2));
    // }
  }
}
