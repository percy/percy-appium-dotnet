using System;
using System.Drawing;

namespace PercyIO.Appium
{
  internal class PercyAppiumElement
  {
    private object element;
    public Size Size { get; set; }
    public Point Location { get; set; }

    internal PercyAppiumElement(object element)
    {
      this.element = element;
      this.Size = GetSize();
      this.Location = GetLocation();
    }

    private Size GetSize()
    {
      return ReflectionUtils.PropertyCall<Size>(element, "Size")!;
    }

    private Point GetLocation()
    {
      return ReflectionUtils.PropertyCall<Point>(element, "Location")!;
    }

    public String Type()
    {
      return ReflectionUtils.MethodCall<String>(element, "GetAttribute", "class")!;
    }
  }
}
