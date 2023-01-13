using System;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Xunit;
using PercyIO.Appium;
namespace Percy.Tests
{
  public class TileTest
  {
    private readonly Tile tile;
    int statusBarHeight = Faker.RandomNumber.Next(50, 200);
    int navigationBarHeight = Faker.RandomNumber.Next(50, 200);
    int headerHeight = Faker.RandomNumber.Next(50, 200);
    int footerHeight = Faker.RandomNumber.Next(50, 200);

    public TileTest()
    {
      tile = new Tile("/tmp", statusBarHeight, navigationBarHeight, headerHeight, footerHeight, false);
    }

    [Fact]
    public void TestGetTilesAsJson()
    {
      // Given
      List<Tile> tileList = new List<Tile>();
      tileList.Add(tile);
      // When
      JObject tileData = Tile.GetTilesAsJson(tileList).ToObject<List<JObject>>()[0];
      // Then
      Assert.Equal(tileData.GetValue("filepath").ToString(), "/tmp");
      Assert.Equal(Convert.ToInt32(tileData.GetValue("statusBarHeight")), statusBarHeight);
      Assert.Equal(Convert.ToInt32(tileData.GetValue("navBarHeight")), navigationBarHeight);
      Assert.Equal(Convert.ToInt32(tileData.GetValue("headerHeight")), headerHeight);
      Assert.Equal(Convert.ToInt32(tileData.GetValue("footerHeight")), footerHeight);
      Assert.False(((bool)tileData.GetValue("fullscreen")));
    }

    [Fact]
    public void TestGetLocalFilePath()
    {
      // Arrange
      String expected = "/tmp";
      // Act
      String actual = tile.LocalFilePath;
      // Assert
      Assert.Equal(expected, actual);
    }

    [Fact]
    public void TestGetStatusBarHeight()
    {
      // Act
      int actual = tile.StatusBarHeight;
      // Assert
      Assert.Equal(statusBarHeight, actual);
    }

    [Fact]
    public void TestGetNavBarHeight()
    {
      // Act
      int actual = tile.NavBarHeight;
      // Assert
      Assert.Equal(navigationBarHeight, actual);
    }

    [Fact]
    public void TestGetHeaderHeight()
    {
      // Act
      int actual = tile.HeaderHeight;
      // Assert
      Assert.Equal(headerHeight, actual);
    }

    [Fact]
    public void TestGetFooterHeight()
    {
      // Act
      int actual = tile.FooterHeight;
      // Assert
      Assert.Equal(footerHeight, actual);
    }

    [Fact]
    public void TestGetFullScreen()
    {
      // Act
      bool actual = tile.FullScreen;
      // Assert
      Assert.False(actual);
    }
  }
}
