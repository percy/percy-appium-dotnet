using System;
using System.Drawing;

namespace PercyIO.Appium
{
  internal class PercyAppiumElement
  {
    private object element;
    internal PercyAppiumElement(object element)
    {
      this.element = element;
    }

    public Size GetSize()
    {
      return (Size)Utils.ReflectionPropertyHelper(element, "Size")!;
    }

    public Point GetLocation()
    {
      return (Point)Utils.ReflectionPropertyHelper(element, "Location")!; 
    }

    public String Type()
    {
      return Utils.ReflectionMethodHelper(element, "GetAttribute", "class")?.ToString()!;
    }
  }
}
