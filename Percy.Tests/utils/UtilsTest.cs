using Xunit;
using PercyIO.Appium;

namespace Percy.Tests
{
  public class UtilsTest
  {
    [Fact]
    public void IsValidDriverObject_ReturnsTrue_ForSupportedDriverType()
    {
      // Arrange — type whose full name contains a supported Appium driver classname
      var driver = new OpenQA.Selenium.Appium.Android.AndroidDriverStub();

      // Act
      bool actual = Utils.isValidDriverObject(driver);

      // Assert
      Assert.True(actual);
    }

    [Fact]
    public void IsValidDriverObject_ReturnsFalse_ForUnsupportedDriverType()
    {
      // Act
      bool actual = Utils.isValidDriverObject("just a string");

      // Assert
      Assert.False(actual);
    }
  }
}

namespace OpenQA.Selenium.Appium.Android
{
  // Full name "OpenQA.Selenium.Appium.Android.AndroidDriverStub" contains the supported
  // classname "OpenQA.Selenium.Appium.Android.AndroidDriver" from Utils.SupportedDriverClassnames,
  // exercising the isValidDriverObject true branch without depending on a real Appium driver.
  internal class AndroidDriverStub
  {
  }
}
