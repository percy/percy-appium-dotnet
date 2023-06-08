using System;

namespace Percy.Tests
{
  internal class TestHelper
  {
    public static void UnsetEnvVariables()
    {
      // Unsert percy loglevel
      Environment.SetEnvironmentVariable("PERCY_LOGLEVEL", null);
    }
  }
}
