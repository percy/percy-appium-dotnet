using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using Moq;
using Xunit;
using PercyIO.Appium;
namespace Percy.Tests
{
  public class AndroidMetadataTest
  {
    private AndroidMetadata? androidMetadata;

    [Fact]
    public void TestGetDeviceName_WhenNameIsNotNull()
    {
      // Arrange
      var expected = "Samsung_gs22u";
      var _androidPercyAppiumDriver = new Mock<IPercyAppiumDriver>();
      androidMetadata = new AndroidMetadata(_androidPercyAppiumDriver.Object, "Samsung_gs22u", 0, 0, null, null);
      // Act
      var actual = androidMetadata.DeviceName();

      // Assert
      Assert.Equal(expected, actual);
    }

    [Fact]
    public void TestGetDeviceName_WhenNameIsNull()
    {
      // Arrange
      var deviceDetail = new Dictionary<string, object>(){
        {"deviceName", "Samsung_gs22u"}
      };
      var capabilities = new Mock<ICapabilities>();
      var _androidPercyAppiumDriver = new Mock<IPercyAppiumDriver>();
      capabilities.Setup(x => x.GetCapability("desired"))
        .Returns(deviceDetail);
      _androidPercyAppiumDriver.Setup(x => x.GetCapabilities())
        .Returns(capabilities.Object);
      var expected = "Samsung_gs22u";
      androidMetadata = new AndroidMetadata(_androidPercyAppiumDriver.Object, null, 0, 0, null, null);
      // Act
      var actual = androidMetadata.DeviceName();

      // Assert
      Assert.Equal(expected, actual);
    }

    [Fact]
    public void TestGetDeviceScreenHeight()
    {
      // Arrange
      var expected = 1420;
      var _androidPercyAppiumDriver = new Mock<IPercyAppiumDriver>();
      var capabilities = new Mock<ICapabilities>();
      capabilities.Setup(x => x.GetCapability("deviceScreenSize"))
        .Returns("1280x1420");
      _androidPercyAppiumDriver.Setup(x => x.GetCapabilities())
        .Returns(capabilities.Object);
      androidMetadata = new AndroidMetadata(_androidPercyAppiumDriver.Object, "Samsung_gs22u", 0, 0, null, null);
      // Act
      int actual = androidMetadata.DeviceScreenHeight();

      // Assert
      Assert.Equal(expected, actual);
    }

    [Fact]
    public void TestGetDeviceScreenWidth()
    {
      // Arrange
      var expected = 1280;
      var _androidPercyAppiumDriver = new Mock<IPercyAppiumDriver>();
      var capabilities = new Mock<ICapabilities>();
      capabilities.Setup(x => x.GetCapability("deviceScreenSize"))
        .Returns("1280x1420");
      _androidPercyAppiumDriver.Setup(x => x.GetCapabilities())
        .Returns(capabilities.Object);
      androidMetadata = new AndroidMetadata(_androidPercyAppiumDriver.Object, "Samsung_gs22u", 0, 0, null, null);
      // Act
      int actual = androidMetadata.DeviceScreenWidth();

      // Assert
      Assert.Equal(expected, actual);
    }

    [Fact]
    public void TestNavBarHeight_WhenNavBarHeightIsNotGiven()
    {
      // Arrange
      AppPercy.cache.Clear();
      var expected = 320;
      var _androidPercyAppiumDriver = new Mock<IPercyAppiumDriver>();
      var capabilities = new Mock<ICapabilities>();
      var viewport = new Dictionary<string, object>(){
        {"top", 100L},
        {"height", 1000L}
      };
      capabilities.Setup(x => x.GetCapability("deviceScreenSize"))
        .Returns("1280x1420");
      capabilities.Setup(x => x.GetCapability("viewportRect"))
        .Returns(viewport);
      _androidPercyAppiumDriver.Setup(x => x.GetCapabilities())
        .Returns(capabilities.Object);
      androidMetadata = new AndroidMetadata(_androidPercyAppiumDriver.Object, "Samsung_gs22u", -1, -1, null, null);
      // Act
      var actual = androidMetadata.NavBarHeight();

      // Assert
      Assert.Equal(expected, actual);
    }

    [Fact]
    public void TestNavBarHeight_WhenNavBarHeightIsGiven()
    {
      // Arrange
      var expected = 100;
      var _androidPercyAppiumDriver = new Mock<IPercyAppiumDriver>();
      var capabilities = new Mock<ICapabilities>();
      androidMetadata = new AndroidMetadata(_androidPercyAppiumDriver.Object, "Samsung_gs22u", 0, 100, null, null);
      // Act
      var actual = androidMetadata.NavBarHeight();

      // Assert
      Assert.Equal(expected, actual);
    }

    [Fact]
    public void TestNavStatBarHeight_WhenStatBarHeightIsNotGiven()
    {
      // Arrange
      AppPercy.cache.Clear();
      var expected = 100;
      var _androidPercyAppiumDriver = new Mock<IPercyAppiumDriver>();
      var capabilities = new Mock<ICapabilities>();
      var viewportRect = new Dictionary<string, object>(){
        {"top", 100L}
      };
      capabilities.Setup(x => x.GetCapability("deviceScreenSize"))
        .Returns("1280x1420");
      capabilities.Setup(x => x.GetCapability("viewportRect"))
        .Returns(viewportRect);
      _androidPercyAppiumDriver.Setup(x => x.GetCapabilities())
        .Returns(capabilities.Object);
      androidMetadata = new AndroidMetadata(_androidPercyAppiumDriver.Object, "Samsung_gs22u", -1, -1, null, null);
      // Act
      var actual = androidMetadata.StatBarHeight();

      // Assert
      Assert.Equal(expected, actual);
    }

    [Fact]
    public void TestNavStatBarHeight_WhenStatBarHeightIsGiven()
    {
      // Arrange
      var expected = 100;
      var _androidPercyAppiumDriver = new Mock<IPercyAppiumDriver>();
      var capabilities = new Mock<ICapabilities>();
      androidMetadata = new AndroidMetadata(_androidPercyAppiumDriver.Object, "Samsung_gs22u", 100, -1, null, null);
      // Act
      var actual = androidMetadata.StatBarHeight();

      // Assert
      Assert.Equal(expected, actual);
    }

    [Fact]
    public void TestOsName()
    {
      Assert.Equal(androidMetadata.OsName(), "Android");
    }
  }
}
