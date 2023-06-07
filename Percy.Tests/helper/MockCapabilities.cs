using System.Collections.Generic;

namespace Percy.Tests
{
  public class MockCapabilities
  {
    private Dictionary<string, object> capabilities;
    public Dictionary<string, object> Capabilities
    {
      get
      {
        return capabilities;
      }
      set
      {
        capabilities = value;
      }
    }
  }
}