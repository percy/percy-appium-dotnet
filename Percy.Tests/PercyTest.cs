using System;
using Xunit;
using PercyIO.Appium;
using RichardSzalay.MockHttp;
using System.Net.Http;
using Newtonsoft.Json;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;
using System.IO;
using System.Collections.Generic;
using OpenQA.Selenium.Support.UI;

namespace Percy.Tests
{
  public class PercyTest
  {
    private readonly MockDriverObject mockDriver;
    public PercyTest()
    {
      mockDriver = new MockDriverObject();
      mockDriver.SessionId = "session-1";
      var mockHttp = new MockHttpMessageHandler();
      CliWrapper.Healthcheck = () =>
      {
        return true;
      };
      var obj = new
      {
        success = true,
        type = "automate"
      };
      CliWrapper.setHttpClient(new HttpClient(mockHttp));
      // Setup a respond for the user api (including a wildcard in the URL)
      mockHttp.When("http://localhost:5338/percy/healthcheck")
        .Respond("application/json", JsonConvert.SerializeObject(obj));
    }

    [Fact]
    public void shouldCallPOA()
    {
      TestHelper.UnsetEnvVariables();
      Env.SetSessionType("automate");
      mockDriver.SetCapability(MetadataBuilder.CapabilityBuilder("Android"));
      mockDriver.setCommandExecutor("https://browserstack.com/wd/hub");
      var percy = new PercyIO.Appium.Percy(mockDriver);

      var mockHttp = new MockHttpMessageHandler();
      var obj = new
      {
        success = true,
        version = "1.0",
      };
      mockHttp.Expect("http://localhost:5338/percy/automateScreenshot")
              .Respond("application/json", JsonConvert.SerializeObject(obj));
      CliWrapper.setHttpClient(new HttpClient(mockHttp));
      
      percy.Screenshot("Screenshot 4");
      mockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public void shouldCallAppPercy()
    {
      TestHelper.UnsetEnvVariables();
      Env.SetSessionType("app-percy");
      mockDriver.SetCapability(MetadataBuilder.CapabilityBuilder("Android"));
      mockDriver.setCommandExecutor("https://browserstack.com/wd/hub");
      var percy = new PercyIO.Appium.Percy(mockDriver);

      var mockHttp = new MockHttpMessageHandler();
      var obj = new
      {
        success = true,
        version = "1.0",
      };
      mockHttp.Expect("http://localhost:5338/percy/comparison")
              .Respond("application/json", JsonConvert.SerializeObject(obj));
      CliWrapper.setHttpClient(new HttpClient(mockHttp));
      
      percy.Screenshot("Screenshot 4");
      mockHttp.VerifyNoOutstandingExpectation();
    }
  }
}
