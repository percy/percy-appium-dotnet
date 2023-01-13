using OpenQA.Selenium;
using Moq;
using Xunit;
using PercyIO.Appium;
namespace Percy.Tests
{
  public class ProviderResolverTest
  {
    private Mock<IPercyAppiumDriver> _androidPercyAppiumDriver = new Mock<IPercyAppiumDriver>();
    private Mock<ICapabilities> capabilities = new Mock<ICapabilities>();

    [Fact]
    public void TestResolveProvider_WithBsSupport()
    {
      // Given
      string url = "http://hub-cloud.browserstack.com/wd/hub";
      _androidPercyAppiumDriver.Setup(x => x.GetHost())
        .Returns(url);
      AppAutomate appAutomate = new AppAutomate(_androidPercyAppiumDriver.Object);
      // When
      //GenericProvider provider =;
      // Then
      Assert.Equal(ProviderResolver.ResolveProvider(_androidPercyAppiumDriver.Object).GetType(), appAutomate.GetType());
    }

    [Fact]
    public void TestResolveProvider_WithoutBsSupport()
    {
      // Given
      string url = "http://hub-cloud.abc.com/wd/hub";
      _androidPercyAppiumDriver.Setup(x => x.GetHost())
        .Returns(url);
      // When
      GenericProvider genericProvider = new GenericProvider(_androidPercyAppiumDriver.Object);
      // Then
      Assert.Equal(ProviderResolver.ResolveProvider(_androidPercyAppiumDriver.Object).GetType(), genericProvider.GetType());
    }
  }
}
