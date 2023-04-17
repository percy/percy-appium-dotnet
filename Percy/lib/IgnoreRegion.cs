using System;

namespace PercyIO.Appium
{
  public class IgnoreRegion
  {
    private int _top;
    private int _bottom;
    private int _left;
    private int _right;

    public IgnoreRegion(int top, int bottom, int left, int right)
    {
      if (top < 0 || bottom < 0 || left < 0 || right < 0)
        throw new ArgumentException("Only Positive integer is allowed!");
      _top = top;
      _bottom = bottom;
      _left = left;
      _right = right;
    }

    public int Top
    {
      get
      {
        return _top;
      }
      set
      {
        if (value < 0)
          throw new ArgumentException("Only Positive integer is allowed!");
        _top = value;
      }
    }

    public int Bottom
    {
      get
      {
        return _bottom;
      }
      set
      {
        if (value < 0)
          throw new ArgumentException("Only Positive integer is allowed!");
        _bottom = value;
      }
    }

    public int Left
    {
      get
      {
        return _left;
      }
      set
      {
        if (value < 0)
          throw new ArgumentException("Only Positive integer is allowed!");
        _left = value;
      }
    }

    public int Right
    {
      get
      {
        return _right;
      }
      set
      {
        if (value < 0)
          throw new ArgumentException("Only Positive integer is allowed!");
        _right = value;
      }
    }

    public Boolean IsValid(int height, int width)
    {
      if (_top >= _bottom || _left >= _right)
        return false;

      if (_top >= height || _bottom > height || _left >= width || _right > width)
        return false;
      return true;
    }
  }
}
