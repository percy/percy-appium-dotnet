using System;
using System.Collections.Generic;

namespace PercyIO.Appium
{
  internal class PercyAppiumCapabilites
  {
    private Dictionary<string, object> capabilites;
    internal PercyAppiumCapabilites(Dictionary<string, object> capabilites)
    {
      this.capabilites = capabilites;
    }

    public Object? getValue(String key)
    {
      if(capabilites.ContainsKey(key))
      {
        return capabilites[key];
      }
      return null;
    }

  }
}
