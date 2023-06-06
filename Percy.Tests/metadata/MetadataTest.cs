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
      // Arrange
      var androidPercyAppiumDriver = new PercyAppiumDriver(_androidDriver);
      var metadata = new Mock<Metadata>(androidPercyAppiumDriver, "Android", 0, 0, "portrait", null).Object;
      var expected = "portrait";
      // Act
      var actual = metadata.Orientation();
      // Assert
      Assert.Equal(actual, expected);
    }

    [Fact]
    public void TestOrientaion_WhenLandscape()
    {
      // Arrange
      var androidPercyAppiumDriver = new PercyAppiumDriver(_androidDriver);
      var metadata = new Mock<Metadata>(androidPercyAppiumDriver, "Android", 0, 0, "landscape", null).Object;
      var expected = "landscape";
      // Act
      var actual = metadata.Orientation();
      // Assert
      Assert.Equal(actual, expected);
    }

    [Fact]
    public void TestOrientaion_WhenAuto()
    {
      // Arrange
      Mock<IPercyAppiumDriver> _androidPercyAppiumDriver = new Mock<IPercyAppiumDriver>();
      _androidPercyAppiumDriver.Setup(x => x.Orientation())
        .Returns("landscape");
      var metadata = new Mock<Metadata>(_androidPercyAppiumDriver.Object, "Android", 0, 0, "auto", null).Object;
      var expected = "landscape";
      // Act
      var actual = metadata.Orientation();
      // Assert
      Assert.Equal(actual, expected);
    }

    [Fact]
    public void TestOrientaion_WhenRandom()
    {
      // Arrange
      var androidPercyAppiumDriver = new PercyAppiumDriver(_androidDriver);
      var metadata = new Mock<Metadata>(androidPercyAppiumDriver, "Android", 0, 0, "random", null).Object;
      var expected = "portrait";
      // Act
      var actual = metadata.Orientation();
      // Assert
      Assert.Equal(actual, expected);
    }

    [Fact]
    public void TestOrientaion_WhenOrientationIsNull()
    {
      // Arrange
      Mock<IPercyAppiumDriver> _androidPercyAppiumDriver = new Mock<IPercyAppiumDriver>();
      var metadata = new Mock<Metadata>(_androidPercyAppiumDriver.Object, "Android", 0, 0, null, null).Object;
      var expected = "portrait";
      // Act
      var actual = metadata.Orientation();
      // Assert
      Assert.Equal(actual, expected);
    }

    [Fact]
    public void TestPlatformVersion_WhenNotNull()
    {
      // Arrange
      var androidPercyAppiumDriver = new PercyAppiumDriver(_androidDriver);
      var metadata = new Mock<Metadata>(androidPercyAppiumDriver, "Android", 0, 0, "portrait", "7").Object;
      var expected = "7";
      // Act
      var actual = metadata.PlatformVersion();
      // Assert
      Assert.Equal(actual, expected);
    }

    [Fact]
    public void TestPlatformVersion_WhenNull()
    {
      // Arrange
      Mock<IPercyAppiumDriver> _androidPercyAppiumDriver = MetadataBuilder.mockDriver("Android");
      var metadata = new Mock<Metadata>(_androidPercyAppiumDriver.Object, "Android", 0, 0, null, null).Object;
      String expected = "12.0";
      // Act
      String actual = metadata.PlatformVersion();
      // Assert
      Assert.Equal(actual, expected);
    }
  }
}
