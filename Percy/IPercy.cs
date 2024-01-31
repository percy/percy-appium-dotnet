using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace PercyIO.Appium
{
  public interface IPercy
  {
    JObject Screenshot(String name, ScreenshotOptions? options, Boolean fullScreen);
    JObject? Screenshot(String name, IEnumerable<KeyValuePair<string, object>>? options);
  }
}
