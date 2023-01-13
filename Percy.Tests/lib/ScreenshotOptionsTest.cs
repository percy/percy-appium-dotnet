using System;
using System.Collections.Generic;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium;
using Moq;
using Xunit;
using PercyIO.Appium;
namespace Percy.Tests
{
  public class ScreenshotOptionsTest
  {
    private readonly ScreenshotOptions screenshotOptions = new ScreenshotOptions();

    [Fact]
    public void TestGetAndSet()
    {
      // Given
      screenshotOptions.DeviceName = "Samsung";
      Assert.Equal(screenshotOptions.DeviceName, "Samsung");
      screenshotOptions.StatusBarHeight = 100;
      Assert.Equal(screenshotOptions.StatusBarHeight, 100);
      screenshotOptions.NavBarHeight = 100;
      Assert.Equal(screenshotOptions.NavBarHeight, 100);
      screenshotOptions.Orientation = "landscape";
      Assert.Equal(screenshotOptions.Orientation, "landscape");
    }
  }
}
