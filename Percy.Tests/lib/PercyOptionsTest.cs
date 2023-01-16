using System;
using System.Collections.Generic;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium;
using Moq;
using Xunit;
using PercyIO.Appium;
namespace Percy.Tests
{
  public class PercyOptionsTest
  {
    private PercyOptions percyOptions;
    private Mock<IPercyAppiumDriver> _androidPercyAppiumDriver = new Mock<IPercyAppiumDriver>();

    public Dictionary<string, object> options(string enabled, string ignoreErrors)
    {
      return new Dictionary<string, object>(){
        {"enabled", enabled},
        {"ignoreErrors", ignoreErrors},
      };
    }

    [Fact]
    public void PercyEnabled_WhenOptionsAndEnabledIsNull()
    {
      // Arrange
      var capabilities = new Mock<ICapabilities>();
      capabilities.Setup(x => x.GetCapability("percyOptions"))
        .Returns(null);
      capabilities.Setup(x => x.GetCapability("percy.enabled"))
        .Returns(null);
      _androidPercyAppiumDriver.Setup(x => x.GetCapabilities())
        .Returns(capabilities.Object);
      percyOptions = new PercyOptions(_androidPercyAppiumDriver.Object);
      //Act
      bool actual = percyOptions.PercyEnabled();
      Assert.True(actual);
    }

    [Fact]
    public void PercyEnabled_WhenOptionsAndEnabledIsPresent_WithFalse()
    {
      // Arrange
      AppPercy.cache.Clear();
      var option = options("False", "False");
      var capabilities = new Mock<ICapabilities>();
      capabilities.Setup(x => x.GetCapability("percyOptions"))
        .Returns(option);
      capabilities.Setup(x => x.GetCapability("percy.enabled"))
        .Returns("False");
      _androidPercyAppiumDriver.Setup(x => x.GetCapabilities())
        .Returns(capabilities.Object);
      percyOptions = new PercyOptions(_androidPercyAppiumDriver.Object);
      //Act
      bool actual = percyOptions.PercyEnabled();
      Assert.False(actual);
    }

    [Fact]
    public void PercyEnabled_WhenOptionsAndEnabledIsPresent_WithTrue()
    {
      // Arrange
      AppPercy.cache.Clear();
      var option = options("True", "False");
      var capabilities = new Mock<ICapabilities>();
      capabilities.Setup(x => x.GetCapability("percyOptions"))
        .Returns(option);
      capabilities.Setup(x => x.GetCapability("percy.enabled"))
        .Returns("True");
      _androidPercyAppiumDriver.Setup(x => x.GetCapabilities())
        .Returns(capabilities.Object);
      percyOptions = new PercyOptions(_androidPercyAppiumDriver.Object);
      //Act
      bool actual = percyOptions.PercyEnabled();
      Assert.True(actual);
    }

    [Fact]
    public void SetPercyIgnoreErrors_WhenOptionsAndEnabledIsNotPresent()
    {
      // Arrange
      AppPercy.cache.Clear();
      var capabilities = new Mock<ICapabilities>();
      capabilities.Setup(x => x.GetCapability("percyOptions"))
        .Returns(null);
      _androidPercyAppiumDriver.Setup(x => x.GetCapabilities())
        .Returns(capabilities.Object);
      // Act
      percyOptions = new PercyOptions(_androidPercyAppiumDriver.Object);
      percyOptions.SetPercyIgnoreErrors();
      //Assert
      Assert.True(AppPercy.ignoreErrors);
    }

    [Fact]
    public void SetPercyIgnoreErrors_WhenOptionsAndEnabledIsPresent()
    {
      // Arrange
      AppPercy.cache.Clear();
      var option = options("False", "False");
      var capabilities = new Mock<ICapabilities>();
      capabilities.Setup(x => x.GetCapability("percyOptions"))
        .Returns(option);
      capabilities.Setup(x => x.GetCapability("percy.ignoreErrors"))
        .Returns("False");
      _androidPercyAppiumDriver.Setup(x => x.GetCapabilities())
        .Returns(capabilities.Object);
      //Act
      percyOptions = new PercyOptions(_androidPercyAppiumDriver.Object);
      percyOptions.SetPercyIgnoreErrors();
      //Assert
      Assert.False(AppPercy.ignoreErrors);
    }

    [Fact]
    public void TestJWPWhenValueTypeIsBool()
    {
       // Arrange
      AppPercy.cache.Clear();
      var capabilities = new Mock<ICapabilities>();
      capabilities.Setup(x => x.GetCapability("percyOptions"))
        .Returns(null);
      capabilities.Setup(x => x.GetCapability("percy.ignoreErrors"))
        .Returns(false);
      capabilities.Setup(x => x.GetCapability("percy.enabled"))
        .Returns(false);
      _androidPercyAppiumDriver.Setup(x => x.GetCapabilities())
        .Returns(capabilities.Object);
      //Act
      percyOptions = new PercyOptions(_androidPercyAppiumDriver.Object);
      percyOptions.SetPercyIgnoreErrors();
      bool actual = percyOptions.PercyEnabled();
      //Assert
      Assert.False(AppPercy.ignoreErrors);
      Assert.False(actual);
    }
  }
}
