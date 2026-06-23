using System;
using Xunit;
using PercyIO.Appium;

namespace Percy.Tests
{
  public class EnvTest
  {
    [Fact]
    public void SetAndGetPercyBuildID()
    {
      // Act
      Env.SetPercyBuildID("build-123");

      // Assert
      Assert.Equal("build-123", Env.GetPercyBuildID());
    }

    [Fact]
    public void SetAndGetPercyBuildUrl()
    {
      // Act
      Env.SetPercyBuildUrl("https://percy.io/builds/123");

      // Assert
      Assert.Equal("https://percy.io/builds/123", Env.GetPercyBuildUrl());
    }
  }
}
