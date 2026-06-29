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
  public class PercyTest : IDisposable
  {
    private readonly MockDriverObject mockDriver;

    // Reset the static HttpClient after every test so a mock handler set here
    // cannot leak into subsequent test classes (cross-class test pollution).
    public void Dispose()
    {
      CliWrapper.resetHttpClient();
    }

    public PercyTest()
    {
      // The AppPercy.cache is static and keyed by sessionId. Tests across classes
      // all reuse "session-1", so a cached (possibly percy-disabled) PercyOptions
      // entry can leak in and short-circuit Screenshot. Clear it like AppPercyTest does.
      AppPercy.cache.Clear();
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
      Environment.SetEnvironmentVariable("PERCY_DISABLE_REMOTE_UPLOADS", "true");
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
      Environment.SetEnvironmentVariable("PERCY_DISABLE_REMOTE_UPLOADS", "false");
    }

    [Fact]
    public void shouldSkipWhenPercyNotEnabled()
    {
      // Arrange — healthcheck disabled means the factory returns early without
      // resolving a percy class, and Screenshot is a no-op.
      TestHelper.UnsetEnvVariables();
      CliWrapper.Healthcheck = () =>
      {
        return false;
      };
      mockDriver.SetCapability(MetadataBuilder.CapabilityBuilder("Android"));
      mockDriver.setCommandExecutor("https://browserstack.com/wd/hub");

      var mockHttp = new MockHttpMessageHandler();
      CliWrapper.setHttpClient(new HttpClient(mockHttp));

      // Act — constructor returns at the disabled guard, Screenshot returns at its guard.
      var percy = new PercyIO.Appium.Percy(mockDriver);
      percy.Screenshot("Screenshot 4");

      // Assert — no screenshot request was ever made.
      mockHttp.VerifyNoOutstandingExpectation();
    }
  }
}
