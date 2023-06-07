using System;
using System.Drawing;

namespace Percy.Tests
{
  internal class MockAppiumElement
  {
    public Size Size
    {
      get
      {
        return new Size(100, 200);
      }
    }

    public Point Location
    {
      get
      {
        return new Point(200, 300);
      }
    }

    public String GetAttribute(String attribute)
    {
      return "Button";
    }
  }
}