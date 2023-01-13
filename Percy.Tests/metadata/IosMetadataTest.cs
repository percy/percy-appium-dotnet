using System;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium.iOS;
using Moq;
using Xunit;
using PercyIO.Appium;
using System.Collections.Generic;

namespace Percy.Tests
{
  public class IosMetadataTest
  {
    private IosMetadata iosMetadata;
    private readonly Mock<IPercyAppiumDriver> _iPhonePercyAppiumDriver = new Mock<IPercyAppiumDriver>();
    private Mock<ICapabilities> capabilities = new Mock<ICapabilities>();

    [Fact]
    public void TestGetDeviceName()
    {
      // Arrange
      var expected = "iPhone_11";
      iosMetadata = new IosMetadata(_iPhonePercyAppiumDriver.Object, "iPhone_11", 0, 0, null, null);
      // Act
      var actual = iosMetadata.DeviceName();

      // Assert
      Assert.Equal(expected, actual);
    }

    [Fact]
    public void TestGetDeviceName_WhenNull()
    {
      // Arrange
      capabilities.Setup(x => x.GetCapability("deviceName"))
        .Returns("iPhone_11");
      _iPhonePercyAppiumDriver.Setup(x => x.GetCapabilities())
        .Returns(capabilities.Object);
      var expected = "iPhone_11";
      iosMetadata = new IosMetadata(_iPhonePercyAppiumDriver.Object, null, 0, 0, null, null);
      // Act
      var actual = iosMetadata.DeviceName();

      // Assert
      Assert.Equal(expected, actual);
    }
    [Fact]
    public void TestNavBarHeight_WhenNavBarHeightIsNotGiven()
    {
      // Arrange
      AppPercy.cache.Clear();
      var expected = 0;
      iosMetadata = new IosMetadata(_iPhonePercyAppiumDriver.Object, "iPhone_11", 0, -1, null, null);
      // Act
      var actual = iosMetadata.NavBarHeight();

      // Assert
      Assert.Equal(expected, actual);
    }

    [Fact]
    public void TestNavBarHeight_WhenNavBarHeightIsGiven()
    {
      // Arrange
      var expected = 100;
      iosMetadata = new IosMetadata(_iPhonePercyAppiumDriver.Object, "iPhone_11", 0, 100, null, null);
      // Act
      var actual = iosMetadata.NavBarHeight();

      // Assert
      Assert.Equal(expected, actual);
    }

    [Fact]
    public void TestNavStatBarHeight_WhenStatBarHeightIsGiven()
    {
      // Arrange
      var expected = 100;
      iosMetadata = new IosMetadata(_iPhonePercyAppiumDriver.Object, "iPhone_11", 100, -1, null, null);
      // Act
      var actual = iosMetadata.StatBarHeight();

      // Assert
      Assert.Equal(expected, actual);
    }

    [Fact]
    public void TestOsName()
    {
      // Given
      capabilities.Setup(x => x.GetCapability("platformName"))
         .Returns("ios");
      _iPhonePercyAppiumDriver.Setup(x => x.GetCapabilities())
        .Returns(capabilities.Object);
      iosMetadata = new IosMetadata(_iPhonePercyAppiumDriver.Object, "Samsung_gs22u", 0, 0, null, null);
      var expected = "iOS";
      // When
      var actual = iosMetadata.OsName();
      // Then
      Assert.Equal(actual, expected);
    }

    [Fact]
    public void TestNavStatBarHeight_WhenStatBarHeightIsNotGiven_AndIsPresentInFile()
    {
      // Arrange
      var expected = 100 * 100;
      MetadataHelper.ValueFromStaticDevicesInfo = (statusBarHeight, ios) =>
      {
        return 100;
      };
      iosMetadata = new IosMetadata(_iPhonePercyAppiumDriver.Object, "iPhone_11", -1, -1, null, null);
      // Act
      var actual = iosMetadata.StatBarHeight();

      // Assert
      Assert.Equal(expected, actual);
    }

    [Fact]
    public void TestNavStatBarHeight_WhenStatBarHeightIsNotGiven_AndIsNotPresentInFile()
    {
      // Arrange
      AppPercy.cache.Clear();
      var expected = 100;
      MetadataHelper.ValueFromStaticDevicesInfo = (statusBarHeight, ios) =>
      {
        return 0;
      };
      var info = new Dictionary<string, object>(){
        { "viewportRect", new Dictionary<string, object> {
          {"top", 100l},
          {"height", 1000l},
          {"width", 400l},
      }}};
      _iPhonePercyAppiumDriver.Setup(x => x.GetSessionDetails())
        .Returns(info);
      iosMetadata = new IosMetadata(_iPhonePercyAppiumDriver.Object, "iPhone_11", -1, -1, null, null);
      // Act
      var actual = iosMetadata.StatBarHeight();

      // Assert
      Assert.Equal(expected, actual);
      AppPercy.cache.Clear();
    }

    [Fact]
    public void TestMetaDataHelperResolver()
    {
      // Arrange
      capabilities.Setup(x => x.GetCapability("platformName"))
         .Returns("iphone");
      capabilities.Setup(x => x.GetCapability("platformVersion"))
        .Returns("9");
      capabilities.Setup(x => x.GetCapability("deviceScreenSize"))
        .Returns("1280x1420");
      capabilities.Setup(x => x.GetCapability("orientation"))
        .Returns("landscape");
      _iPhonePercyAppiumDriver.Setup(x => x.GetCapabilities())
        .Returns(capabilities.Object);
      _iPhonePercyAppiumDriver.Setup(x => x.GetType())
        .Returns("iOS");
      Type type = MetadataHelper.Resolve(_iPhonePercyAppiumDriver.Object, "iphone", 100, 200, null, null).GetType();
      // Act
      iosMetadata = new IosMetadata(_iPhonePercyAppiumDriver.Object, "iPhone_11", -1, -1, null, null);
      // Assert
      Assert.Equal(type, iosMetadata.GetType());
    }

    [Fact]
    public void TestDeviceScreenHeight()
    {
      // Arrange
      var expected = 1000;
      MetadataHelper.ValueFromStaticDevicesInfo = (statusBarHeight, ios) =>
      {
        return 1000;
      };
      iosMetadata = new IosMetadata(_iPhonePercyAppiumDriver.Object, "iPhone_11", -1, -1, null, null);
      // Act
      var actual = iosMetadata.DeviceScreenHeight();

      // Assert
      Assert.Equal(expected, actual);
    }

    [Fact]
    public void TestDeviceScreenHeight_WhenNotFoundInFile()
    {
      // Arrange
      AppPercy.cache.Clear();
      var expected = 1000;
      MetadataHelper.ValueFromStaticDevicesInfo = (statusBarHeight, ios) =>
      {
        return 0;
      };
      var info = new Dictionary<string, object>(){
        { "viewportRect", new Dictionary<string, object> {
          {"top", 100l},
          {"height", 1000l},
          {"width", 400l},
      }}};
      _iPhonePercyAppiumDriver.Setup(x => x.GetSessionDetails())
        .Returns(info);
      iosMetadata = new IosMetadata(_iPhonePercyAppiumDriver.Object, "iPhone_11", -1, -1, null, null);
      // Act
      var actual = iosMetadata.DeviceScreenHeight();

      // Assert
      Assert.Equal(expected, actual);
    }

    [Fact]
    public void TestDeviceScreenWidth_WhenNotFoundInFile()
    {
      // Arrange
      AppPercy.cache.Clear();
      var expected = 400;
      MetadataHelper.ValueFromStaticDevicesInfo = (statusBarHeight, ios) =>
      {
        return 0;
      };
      var info = new Dictionary<string, object>(){
        {"viewportRect", new Dictionary<string, object> {
          {"top", 100l},
          {"height", 1000l},
          {"width", 400l},
      }}};
      _iPhonePercyAppiumDriver.Setup(x => x.GetSessionDetails())
        .Returns(info);
      iosMetadata = new IosMetadata(_iPhonePercyAppiumDriver.Object, "iPhone_11", -1, -1, null, null);
      // Act
      var actual = iosMetadata.DeviceScreenWidth();

      // Assert
      Assert.Equal(expected, actual);
    }

    [Fact]
    public void TestDeviceScreenWidth()
    {
      // Arrange
      int expected = 300;
      MetadataHelper.ValueFromStaticDevicesInfo = (statusBarHeight, ios) =>
        {
          return 300;
        };
      iosMetadata = new IosMetadata(_iPhonePercyAppiumDriver.Object, "iPhone_11", -1, -1, null, null);
      // Act
      int actual = iosMetadata.DeviceScreenWidth();

      // Assert
      Assert.Equal(expected, actual);
    }
  }
}
