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
      // Arrange
      Mock<IPercyAppiumDriver> _androidPercyAppiumDriver = new Mock<IPercyAppiumDriver>();
      Mock<ICapabilities> capabilities = new Mock<ICapabilities>();
      string url = "http://hub-cloud.browserstack.com/wd/hub";
      _androidPercyAppiumDriver.Setup(x => x.GetHost())
        .Returns(url);
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
      AppPercy appPercy = new AppPercy(_androidPercyAppiumDriver.Object);
      CliWrapper.Healthcheck = () => {
        return true;
      };
      // Act
      appPercy.Screenshot("abc", null);
      // Assert
      _androidPercyAppiumDriver.Verify(x => x.ExecuteScript(It.IsAny<string>()), Times.Exactly(2));
    }

    [Fact]
    public void TestName_WithiOS()
    {
      // Arrange
      Mock<IPercyAppiumDriver> _iOSPercyAppiumDriver = new Mock<IPercyAppiumDriver>();
      Mock<ICapabilities> capabilities = new Mock<ICapabilities>();
      string url = "http://hub-cloud.browserstack.com/wd/hub";
      _iOSPercyAppiumDriver.Setup(x => x.GetHost())
        .Returns(url);
      _iOSPercyAppiumDriver.Setup(x => x.GetCapabilities())
        .Returns(capabilities.Object);
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
      AppPercy appPercy = new AppPercy(_iOSPercyAppiumDriver.Object);
      CliWrapper.Healthcheck = () => {
        return true;
      };
      // Act
      appPercy.Screenshot("abc", null);
      // Assert
      _iOSPercyAppiumDriver.Verify(x => x.ExecuteScript(It.IsAny<string>()), Times.Exactly(2));
    }
  }
}
