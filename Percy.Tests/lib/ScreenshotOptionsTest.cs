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
      screenshotOptions.FullScreen = true;
      Assert.True(screenshotOptions.FullScreen);
      screenshotOptions.FullPage = true;
      Assert.True(screenshotOptions.FullPage);
      screenshotOptions.ScreenLengths = 2;
      Assert.Equal(screenshotOptions.ScreenLengths, 2);
      screenshotOptions.ScrollableXpath = "some/xpath";
      Assert.Equal(screenshotOptions.ScrollableXpath, "some/xpath");
      screenshotOptions.ScrollableId = "someId";
      Assert.Equal(screenshotOptions.ScrollableId, "someId");
      Assert.Equal(screenshotOptions.IgnoreRegionAccessibilityIds.Count, 0);
      Assert.Equal(screenshotOptions.IgnoreRegionXpaths.Count, 0);
      Assert.Equal(screenshotOptions.IgnoreRegionXpaths.Count, 0);
      Assert.Equal(screenshotOptions.CustomIgnoreRegions.Count, 0);
    }
  }
}
