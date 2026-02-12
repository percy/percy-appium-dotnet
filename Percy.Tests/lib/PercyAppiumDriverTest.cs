using System;
using Xunit;
using PercyIO.Appium;
using System.Reflection;

namespace Percy.Tests
{
  public class PercyAppiumDriverTest
  {
    [Fact]
    public void TestGetHost_V4()
    {
      var mockDriver = new MockDriverObject();
      mockDriver.setIsV5(false);
      mockDriver.setCommandExecutor("http://browserstack.com/wd/hub");
      
      var percyAppiumDriver = new PercyAppiumDriver(mockDriver);
      var host = percyAppiumDriver.GetHost();
      
      Assert.Equal("http://browserstack.com/wd/hub", host);
    }

    [Fact]
    public void TestGetHost_V5()
    {
      var mockDriver = new MockDriverObject();
      mockDriver.setIsV5(true);
      mockDriver.setCommandExecutor("http://browserstack.com/wd/hub");
      
      var percyAppiumDriver = new PercyAppiumDriver(mockDriver);
      var host = percyAppiumDriver.GetHost();
      
      Assert.Equal("http://browserstack.com/wd/hub", host);
    }

    [Fact]
    public void TestGetHost_Null()
    {
       // Test case where extraction fails (returns null)
       var mockDriver = new Object(); // Plain object, no CommandExecutor
       var percyAppiumDriver = new PercyAppiumDriver(mockDriver);
       var host = percyAppiumDriver.GetHost();
       
       Assert.Null(host);
    }
  }
}
