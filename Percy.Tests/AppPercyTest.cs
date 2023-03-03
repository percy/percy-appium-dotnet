using System;
using System.Collections.Generic;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium;
using Moq;
using Xunit;
using PercyIO.Appium;
using OpenQA.Selenium.Appium.Android;
using Newtonsoft.Json.Linq;

namespace Percy.Tests
{
  public class AppPercyTest
  {

    [Fact]
    public void TestName_WithAndroid()
    {
      AppPercy.cache.Clear();
      // Arrange
      Mock<IPercyAppiumDriver> _androidPercyAppiumDriver = new Mock<IPercyAppiumDriver>();
      Mock<ICapabilities> capabilities = new Mock<ICapabilities>();
      var info = new Dictionary<string, object>(){
          {"top", 100l},
          {"height", 1000l},
          {"width", 400l},
      };
      capabilities.Setup(x => x.GetCapability("platformName"))
         .Returns("android");
      capabilities.Setup(x => x.GetCapability("deviceScreenSize"))
        .Returns("1280x1420");
      capabilities.Setup(x => x.GetCapability("viewportRect"))
        .Returns(info);
      string url = "http://hub-cloud.browserstack.com/wd/hub";
      _androidPercyAppiumDriver.Setup(x => x.GetHost())
        .Returns(url);
      _androidPercyAppiumDriver.Setup(x => x.GetType())
        .Returns("Android");
      _androidPercyAppiumDriver.Setup(x => x.GetCapabilities())
        .Returns(capabilities.Object);

      var arguments = new JObject();
      var response = @"{
        success:'true',
        osVersion:'11.2',
        buildHash:'abc',
        sessionHash:'def',
        deviceName: 'Android'
      }";
      _androidPercyAppiumDriver.Setup(x => x.sessionId())
        .Returns(new SessionId("abc").ToString());
      _androidPercyAppiumDriver.Setup(x => x.GetType())
        .Returns("Android");
      _androidPercyAppiumDriver.Setup(x => x.ExecuteScript(It.IsAny<string>()))
        .Returns(response);
      var screenshot = new Screenshot("c2hvcnRlc3Q=");
      _androidPercyAppiumDriver.Setup(x => x.GetScreenshot())
        .Returns(screenshot);
      CliWrapper.Healthcheck = () => {
        return true;
      };

      AppPercy appPercy = new AppPercy(_androidPercyAppiumDriver.Object);

      // Act
      appPercy.Screenshot("abc", null);
      // Assert
      _androidPercyAppiumDriver.Verify(x => x.ExecuteScript(It.IsAny<string>()), Times.Exactly(2));
    }

    [Fact]
    public void TestName_WithiOS()
    {
      AppPercy.cache.Clear();
      // Arrange
      Mock<IPercyAppiumDriver> _iOSPercyAppiumDriver = new Mock<IPercyAppiumDriver>();
      Mock<ICapabilities> capabilities = new Mock<ICapabilities>();
      capabilities.Setup(x => x.GetCapability("platformName"))
         .Returns("ios");
      capabilities.Setup(x => x.GetCapability("deviceScreenSize"))
        .Returns("1280x1420");
      string url = "http://hub-cloud.browserstack.com/wd/hub";
      _iOSPercyAppiumDriver.Setup(x => x.GetHost())
        .Returns(url);
      _iOSPercyAppiumDriver.Setup(x => x.GetType())
        .Returns("iOS");
      _iOSPercyAppiumDriver.Setup(x => x.GetCapabilities())
        .Returns(capabilities.Object);
      var info = new Dictionary<string, object>(){
        { "viewportRect", new Dictionary<string, object> {
          {"top", 100l},
          {"height", 1000l},
          {"width", 400l},
      }}};
      var screenshot = new Screenshot("c2hvcnRlc3Q=");
      _iOSPercyAppiumDriver.Setup(x => x.GetScreenshot())
        .Returns(screenshot);
      _iOSPercyAppiumDriver.Setup(x => x.GetSessionDetails())
        .Returns(info);
       var arguments = new JObject();
       var response = @"{
        success:'true',
        osVersion:'11.2',
        buildHash:'abc',
        sessionHash:'def',
        deviceName: 'iPhone'
      }";
      _iOSPercyAppiumDriver.Setup(x => x.sessionId())
        .Returns(new SessionId("abc").ToString());
      _iOSPercyAppiumDriver.Setup(x => x.GetType())
        .Returns("iOS");
      _iOSPercyAppiumDriver.Setup(x => x.ExecuteScript(It.IsAny<string>()))
        .Returns(response);
      CliWrapper.Healthcheck = () => {
        return true;
      };

      AppPercy appPercy = new AppPercy(_iOSPercyAppiumDriver.Object);
      
      // Act
      appPercy.Screenshot("abc", null);
      // Assert
      _iOSPercyAppiumDriver.Verify(x => x.ExecuteScript(It.IsAny<string>()), Times.Exactly(2));
    }
  }
}
