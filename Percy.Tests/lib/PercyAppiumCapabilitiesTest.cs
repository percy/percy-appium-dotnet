using System;
using System.Collections.Generic;
using Xunit;
using PercyIO.Appium;

namespace Percy.Tests.lib
{
  public class PercyAppiumCapabilitiesTest
  {
    [Fact]
    public void TestGetCapability_WithStandardField()
    {
       var mockDriver = new MockDriverWithStandardCapabilities();
       var percyCaps = new PercyAppiumCapabilities(mockDriver);
       var caps = percyCaps.GetCapabilities();
       Assert.NotNull(caps);
       Assert.True(caps.ContainsKey("platformName"));
    }

    [Fact]
    public void TestGetCapability_WithDifferentFieldNaming()
    {
       var mockDriver = new MockDriverWithV5Capabilities();
       var percyCaps = new PercyAppiumCapabilities(mockDriver);
       var caps = percyCaps.GetCapabilities();
       
       Assert.NotNull(caps);
       Assert.True(caps.ContainsKey("platformName"));
       Assert.Equal("AndroidV5", caps["platformName"]);
    }
  }

  internal class MockDriverWithStandardCapabilities
  {
      public MockStandardCapabilities Capabilities { get; } = new MockStandardCapabilities();
  }

  internal class MockStandardCapabilities
  {
      private Dictionary<string, object> capabilities = new Dictionary<string, object> 
      { 
          { "platformName", "Android" } 
      };
      
      public Dictionary<string, object> AsDictionary() => capabilities;
  }

  internal class MockDriverWithV5Capabilities
  {
      public MockV5Capabilities Capabilities { get; } = new MockV5Capabilities();
  }

  internal class MockV5Capabilities
  {
      // Renamed backing field, simulating internal change (match code: _capabilities)
      private Dictionary<string, object> _capabilities = new Dictionary<string, object> 
      { 
          { "platformName", "AndroidV5" } 
      };
  }
}
