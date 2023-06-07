using System;

namespace Percy.Tests
{
  internal class TestHelper
  {
    public static void UnsetEnvVariables()
    {
      // Get the list of environment variables
      var envVariables = Environment.GetEnvironmentVariables();

      // Unset each environment variable
      foreach (var envVarKey in envVariables.Keys)
      {
        string key = envVarKey.ToString();
        Environment.SetEnvironmentVariable(key, null);
      }
    }
  }
}
