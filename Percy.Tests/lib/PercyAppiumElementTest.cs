using System.Drawing;
using PercyIO.Appium;
using Xunit;

namespace Percy.Tests
{
  public class PercyAppiumElementTest
  {
    [Fact]
    public void Test()
    {
      // Arrange
      var appiumElement = new MockAppiumElement();
      // Act
      var percyAppiumElement = new PercyAppiumElement(appiumElement);
      var expectedSize = new Size(100, 200);
      var expectedLocation = new Point(200, 300);
      var expectedType = "Button";
      var expectedId = "element_id";
      // Assert
      Assert.Equal(expectedSize, percyAppiumElement.Size);
      Assert.Equal(expectedLocation, percyAppiumElement.Location);
      Assert.Equal(expectedType, percyAppiumElement.Type());
      Assert.Equal(percyAppiumElement.id, expectedId);
    }
  }
}
