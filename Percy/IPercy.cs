using System;
using System.Collections.Generic;

namespace PercyIO.Appium
{
  public interface IPercy
  {
    void Screenshot(String name, ScreenshotOptions? options, Boolean fullScreen);
    void Screenshot(String name, IEnumerable<KeyValuePair<string, object>>? options);
  }
}
