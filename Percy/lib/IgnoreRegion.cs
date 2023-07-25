using System;

namespace PercyIO.Appium
{
  public class IgnoreRegion : Region
  {
     public IgnoreRegion(int top, int bottom, int left, int right) : base(top, bottom, left, right) 
     {

     }
  }
}
