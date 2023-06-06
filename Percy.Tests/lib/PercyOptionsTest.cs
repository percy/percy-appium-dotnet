using System;
using Xunit;
using PercyIO.Appium;
using System.Dynamic;

namespace Percy.Tests
{
  public class PercyOptionsTest
  {

    public dynamic createPercyOption(bool jwp, String percyEnabled, String ignoreError)
    {
      dynamic percyOption = new ExpandoObject();
      percyOption.jwp = jwp;
      percyOption.percyEnabled = percyEnabled;
      percyOption.ignoreError = ignoreError;
      return percyOption;
    }

    [Fact]
    public void PercyEnabled_WhenOptionsAndEnabledIsNull()
    {
      // Arrange
      var _androidPercyAppiumDriver = MetadataBuilder.mockDriver("Android");
      var percyOptions = new PercyOptions(_androidPercyAppiumDriver.Object);
      //Act
      bool actual = percyOptions.PercyEnabled();
      Assert.True(actual);
    }

    [Fact]
    public void PercyEnabled_WhenOptionsAndEnabledIsPresent_WithFalse()
    {
      // Arrange
      AppPercy.cache.Clear();
      var percyOption = createPercyOption(true, "False", "False");
      var _androidPercyAppiumDriver = MetadataBuilder.mockDriver("Android", percyOption);
      var percyOptions = new PercyOptions(_androidPercyAppiumDriver.Object);
      //Act
      bool actual = percyOptions.PercyEnabled();
      Assert.False(actual);
    }

    [Fact]
    public void PercyEnabled_WhenOptionsAndEnabledIsPresent_WithTrue()
    {
      // Arrange
      AppPercy.cache.Clear();
      var percyOption = createPercyOption(true, "True", "True");
      
      var _androidPercyAppiumDriver = MetadataBuilder.mockDriver("Android", percyOption);
      var percyOptions = new PercyOptions(_androidPercyAppiumDriver.Object);
      //Act
      bool actual = percyOptions.PercyEnabled();
      Assert.True(actual);
    }

    [Fact]
    public void SetPercyIgnoreErrors_WhenOptionsAndEnabledIsNotPresent()
    {
      // Arrange
      AppPercy.cache.Clear();
      var _androidPercyAppiumDriver = MetadataBuilder.mockDriver("Android");
      
      // Act
      var percyOptions = new PercyOptions(_androidPercyAppiumDriver.Object);
      percyOptions.SetPercyIgnoreErrors();
      //Assert
      Assert.True(AppPercy.ignoreErrors);
    }

    [Fact]
    public void SetPercyIgnoreErrors_WhenOptionsAndEnabledIsPresent()
    {
      // Arrange
      AppPercy.cache.Clear();
      var percyOption = createPercyOption(true, "False", "False");
      var _androidPercyAppiumDriver = MetadataBuilder.mockDriver("Android", percyOption);
      //Act
      var percyOptions = new PercyOptions(_androidPercyAppiumDriver.Object);
      percyOptions.SetPercyIgnoreErrors();
      //Assert
      Assert.False(AppPercy.ignoreErrors);
    }
  }
}
