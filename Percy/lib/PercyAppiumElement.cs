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
      return RefectionUtils.PropertyCall<Size>(element, "Size")!;
    }

    public Point GetLocation()
    {
      return RefectionUtils.PropertyCall<Point>(element, "Location")!; 
    }

    public String Type()
    {
      return RefectionUtils.MethodCall<String>(element, "GetAttribute", "class")!;
    }
  }
}
