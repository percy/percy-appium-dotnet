using System;
using System.Collections.Generic;

namespace PercyIO.Appium
{
  internal interface IPercyAppiumCapabilities
  {
    T getValue<T>(string key);
    Dictionary<string, object> GetCapability(Object driver);
    void SetCapability(Dictionary<string, object> capabilities);
  }
}
