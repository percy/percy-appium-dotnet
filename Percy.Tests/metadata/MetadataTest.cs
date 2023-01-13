using System;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium;
using Moq;
using Xunit;
using PercyIO.Appium;
namespace Percy.Tests
{
  public class MetadataTest
  {
    AndroidDriver<AndroidElement> _androidDriver;

    [Fact]
    public void GetType_ShouldGetAndroidValue_WhenAndroid()
    {
      // Arrange
      var androidPercyAppiumDriver = new PercyAppiumDriver(_androidDriver);
      var metadata = new Mock<Metadata>(androidPercyAppiumDriver, "Android", 0, 0, null, null).Object;
      var c = "Android";
      // Act
      var a = metadata.GetDeviceName();

      // Assert
      Assert.Equal(c, a);
    }

    [Fact]
    public void TestOrientaion_WhenPotrait()
    {
      // Given
      var androidPercyAppiumDriver = new PercyAppiumDriver(_androidDriver);
      var metadata = new Mock<Metadata>(androidPercyAppiumDriver, "Android", 0, 0, "portrait", null).Object;
      var expected = "portrait";
      // When
      var actual = metadata.Orientation();
      // Then
      Assert.Equal(actual, expected);
    }

    [Fact]
    public void TestOrientaion_WhenLandscape()
    {
      // Given
      var androidPercyAppiumDriver = new PercyAppiumDriver(_androidDriver);
      var metadata = new Mock<Metadata>(androidPercyAppiumDriver, "Android", 0, 0, "landscape", null).Object;
      var expected = "landscape";
      // When
      var actual = metadata.Orientation();
      // Then
      Assert.Equal(actual, expected);
    }

    [Fact]
    public void TestOrientaion_WhenAuto()
    {
      // Given
      Mock<IPercyAppiumDriver> _androidPercyAppiumDriver = new Mock<IPercyAppiumDriver>();
      _androidPercyAppiumDriver.Setup(x => x.Orientation())
        .Returns("landscape");
      var metadata = new Mock<Metadata>(_androidPercyAppiumDriver.Object, "Android", 0, 0, "auto", null).Object;
      var expected = "landscape";
      // When
      var actual = metadata.Orientation();
      // Then
      Assert.Equal(actual, expected);
    }

    [Fact]
    public void TestOrientaion_WhenRandom()
    {
      // Given
      var androidPercyAppiumDriver = new PercyAppiumDriver(_androidDriver);
      var metadata = new Mock<Metadata>(androidPercyAppiumDriver, "Android", 0, 0, "random", null).Object;
      var expected = "portrait";
      // When
      var actual = metadata.Orientation();
      // Then
      Assert.Equal(actual, expected);
    }

    [Fact]
    public void TestOrientaion_WhenOrientationIsNull()
    {
      // Given
      Mock<ICapabilities> capabilities = new Mock<ICapabilities>();
      capabilities.Setup(x => x.GetCapability("orientation"))
        .Returns(null);
      Mock<IPercyAppiumDriver> _androidPercyAppiumDriver = new Mock<IPercyAppiumDriver>();
      _androidPercyAppiumDriver.Setup(x => x.GetCapabilities())
        .Returns(capabilities.Object);
      var metadata = new Mock<Metadata>(_androidPercyAppiumDriver.Object, "Android", 0, 0, null, null).Object;
      var expected = "portrait";
      // When
      var actual = metadata.Orientation();
      // Then
      Assert.Equal(actual, expected);
    }

    [Fact]
    public void TestPlatformVersion_WhenNotNull()
    {
      // Given
      var androidPercyAppiumDriver = new PercyAppiumDriver(_androidDriver);
      var metadata = new Mock<Metadata>(androidPercyAppiumDriver, "Android", 0, 0, "portrait", "7").Object;
      var expected = "7";
      // When
      var actual = metadata.PlatformVersion();
      // Then
      Assert.Equal(actual, expected);
    }

    [Fact]
    public void TestPlatformVersion_WhenNull()
    {
      // Given
      Mock<ICapabilities> capabilities = new Mock<ICapabilities>();
      capabilities.Setup(x => x.GetCapability("platformVersion"))
        .Returns(null);
      capabilities.Setup(x => x.GetCapability("os_version"))
        .Returns(null);
      Mock<IPercyAppiumDriver> _androidPercyAppiumDriver = new Mock<IPercyAppiumDriver>();
      _androidPercyAppiumDriver.Setup(x => x.GetCapabilities())
        .Returns(capabilities.Object);
      var metadata = new Mock<Metadata>(_androidPercyAppiumDriver.Object, "Android", 0, 0, null, null).Object;
      String expected = null;
      // When
      String actual = metadata.PlatformVersion();
      // Then
      Assert.Equal(actual, expected);
    }
  }
}
