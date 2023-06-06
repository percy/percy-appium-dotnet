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
    private Mock<IPercyAppiumDriver> _androidPercyAppiumDriver = new Mock<IPercyAppiumDriver>();

    public AndroidMetadataTest()
    {
      _androidPercyAppiumDriver = MetadataBuilder.mockDriver("Android");
    }
    [Fact]
    public void TestGetDeviceName_WhenNameIsNotNull()
    {
      
      // Arrange
      var expected = "Samsung_gs22u";
      androidMetadata = new AndroidMetadata(_androidPercyAppiumDriver.Object, "Samsung_gs22u", 0, 0, null, null);
      // Act
      var actual = androidMetadata.DeviceName();

      // Assert
      Assert.Equal(expected, actual);
    }

    [Fact]
    public void TestGetDeviceName_WhenNameIsNull()
    { 
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
      androidMetadata = new AndroidMetadata(_androidPercyAppiumDriver.Object, "Samsung_gs22u", 0, 100, null, null);
      Assert.Equal(androidMetadata.OsName(), "Android");
    }
  }
}
