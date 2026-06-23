using System;
using Moq;
using Xunit;
using PercyIO.Appium;
namespace Percy.Tests
{
  public class MetadataHelperTest
  {
    [Fact]
    public void TestResolve_ReturnsAndroidMetadata_WhenAndroidDriver()
    {
      // Arrange: driver class name contains "Android" (MetadataHelper lines 16-19)
      Mock<IPercyAppiumDriver> _androidPercyAppiumDriver = MetadataBuilder.mockDriver("Android");
      // Act
      var metadata = MetadataHelper.Resolve(_androidPercyAppiumDriver.Object, "Samsung_gs22u", 100, 200, null, null);
      // Assert
      Assert.NotNull(metadata);
      Assert.IsType<AndroidMetadata>(metadata);
    }

    [Fact]
    public void TestResolve_ReturnsIosMetadata_WhenIosDriver()
    {
      // Arrange: driver class name contains "iOS" (MetadataHelper lines 21-23)
      Mock<IPercyAppiumDriver> _iosPercyAppiumDriver = MetadataBuilder.mockDriver("iOS");
      // Act
      var metadata = MetadataHelper.Resolve(_iosPercyAppiumDriver.Object, "iPhone_11", 100, 200, null, null);
      // Assert
      Assert.NotNull(metadata);
      Assert.IsType<IosMetadata>(metadata);
    }

    [Fact]
    public void TestResolve_ReturnsNull_WhenUnsupportedDriver()
    {
      // Arrange: driver class name is neither Android nor iOS, so the else branch
      // throws, is caught, logged, and null is returned
      // (MetadataHelper lines 26, 27, 30, 31, 32, 33, 34)
      Mock<IPercyAppiumDriver> _unknownPercyAppiumDriver = new Mock<IPercyAppiumDriver>();
      _unknownPercyAppiumDriver.Setup(x => x.GetType()).Returns("Windows");
      // Act
      var metadata = MetadataHelper.Resolve(_unknownPercyAppiumDriver.Object, "device", 100, 200, null, null);
      // Assert
      Assert.Null(metadata);
    }

    [Fact]
    public void TestValueFromStaticDevicesInfo_WhenDevicePresentInFile()
    {
      // Arrange: a device present in resources/devices.json should resolve the
      // requested key from the embedded JSON, exercising the real delegate and
      // GetDevicesJson (MetadataHelper lines 38, 39, 44, 48, 49, 50, 51, 52, 53, 54, 55, 56)
      AppPercy.cache.Clear();
      var expected = 44; // "iphone x" -> statusBarHeight
      // Act
      var actual = MetadataHelper.ValueFromStaticDevicesInfo("statusBarHeight", "iphone x");
      // Assert
      Assert.Equal(expected, actual);
    }

    [Fact]
    public void TestValueFromStaticDevicesInfo_WhenDeviceAbsentInFile()
    {
      // Arrange: a device that is not present returns 0 (MetadataHelper lines 39, 40, 41, 42)
      // Uses the cached JSON populated by the previous lookup as well as a fresh read.
      var expected = 0;
      // Act
      var actual = MetadataHelper.ValueFromStaticDevicesInfo("statusBarHeight", "non_existent_device");
      // Assert
      Assert.Equal(expected, actual);
    }
  }
}
