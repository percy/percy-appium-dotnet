using System;
using PercyIO.Appium;
using Xunit;

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
}
