using System;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium.iOS;
using Moq;
using Xunit;
using PercyIO.Appium;
namespace Percy.Tests
{
  public class PercyAppiumDriverTest
  {

    private IOSDriver<IOSElement> _iosDriver;
    private AndroidDriver<AndroidElement> _androidDriver;

    [Fact]
    public void GetType_ShouldGetiOSValue_WheniOS()
    {
      // Arrange
      var iosPercyAppiumDriver = new PercyAppiumDriver(_iosDriver);
      var expected = "iOS";
      // Act
      var actual = iosPercyAppiumDriver.GetType();

      // Assert
      Assert.Equal(expected, actual);
    }

    [Fact]
    public void GetType_ShouldGetAndroidValue_WhenAndroid()
    {
      // Arrange
      var androidPercyAppiumDriver = new PercyAppiumDriver(_androidDriver);
      var expected = "Android";
      // Act
      var actual = androidPercyAppiumDriver.GetType();
      // Assert
      Assert.Equal(expected, actual);
    }
  }
}
