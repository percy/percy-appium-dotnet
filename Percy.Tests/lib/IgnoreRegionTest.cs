using System;
using PercyIO.Appium;
using Xunit;

namespace Percy.Tests
{
  public class IgnoreRegionTests
  {
    [Fact]
    public void Top_ShouldThrowArgumentException_WhenGivenNegativeValue()
    {
      var region = new IgnoreRegion();

      Assert.Throws<ArgumentException>(() => region.Top = -1);
    }

    [Fact]
    public void Bottom_ShouldThrowArgumentException_WhenGivenNegativeValue()
    {
      var region = new IgnoreRegion();

      Assert.Throws<ArgumentException>(() => region.Bottom = -1);
    }

    [Fact]
    public void Left_ShouldThrowArgumentException_WhenGivenNegativeValue()
    {
      var region = new IgnoreRegion();

      Assert.Throws<ArgumentException>(() => region.Left = -1);
    }

    [Fact]
    public void Right_ShouldThrowArgumentException_WhenGivenNegativeValue()
    {
      var region = new IgnoreRegion();

      Assert.Throws<ArgumentException>(() => region.Right = -1);
    }

    [Fact]
    public void TestGetAndSet()
    {
      // Given
      var region = new IgnoreRegion();
      // When
      region.Top = 123;
      region.Bottom = 234;
      region.Left = 543;
      region.Right = 789;
      // Then
      Assert.Equal(region.Top, 123);
      Assert.Equal(region.Bottom, 234);
      Assert.Equal(region.Left, 543);
      Assert.Equal(region.Right, 789);
    }

    [Fact]
    public void IsValid_ReturnsTrue_WhenAllPropertiesAreNonNegative()
    {
      // Arrange
      IgnoreRegion region = new IgnoreRegion();
      region.Top = 10;
      region.Bottom = 20;
      region.Left = 30;
      region.Right = 40;
      var width = 1080;
      var height = 2500;

      // Act
      bool isValid = region.IsValid(height, width);

      // Assert
      Assert.True(isValid);
    }

    [Fact]
    public void IsValid_ReturnsFalse_WhenAnyPropertyIsNegative()
    {
      // Arrange
      IgnoreRegion region = new IgnoreRegion();
      region.Top = 10;
      region.Bottom = 2000;
      region.Left = 30;
      region.Right = 1190;

      var width = 1080;
      var height = 2500;

      // Act
      bool isValid = region.IsValid(height, width);

      // Assert
      Assert.False(isValid);
    }
  }
}
