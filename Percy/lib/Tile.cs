using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace PercyIO.Appium
{
  internal class Tile
  {
    // File path where screenshot is stored
    internal String LocalFilePath {get;}
    internal int StatusBarHeight {get;}
    internal int NavBarHeight {get;}
    internal int HeaderHeight {get;} 
    internal int FooterHeight {get;}
    internal bool FullScreen {get;}
    internal String? Sha {get;}
    

    internal Tile(
      String localFilePath,
      int statusBarHeight,
      int navBarHeight,
      int headerHeight,
      int footerHeight,
      bool fullScreen,
      String? sha = null
      )
    {
      this.LocalFilePath = localFilePath;
      this.StatusBarHeight = statusBarHeight;
      this.NavBarHeight = navBarHeight;
      this.HeaderHeight = headerHeight;
      this.FooterHeight = footerHeight;
      this.FullScreen = fullScreen;
      this.Sha = sha;
    }

    internal static JArray GetTilesAsJson(List<Tile> tilesList)
    {
      var tiles = new JArray();
      foreach (var tile in tilesList)
      {
        var tileData = new JObject();
        tileData.Add("filepath", tile.LocalFilePath);
        tileData.Add("statusBarHeight", 0);
        tileData.Add("navBarHeight", 0);
        tileData.Add("headerHeight", tile.HeaderHeight);
        tileData.Add("footerHeight", tile.FooterHeight);
        tileData.Add("fullscreen", tile.FullScreen);
        tileData.Add("sha", tile.Sha);
        tiles.Add(tileData);
      }
      return tiles;
    }
  }
}
