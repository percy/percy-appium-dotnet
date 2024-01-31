using System;
using System.Collections.Generic;
using Xunit;
using PercyIO.Appium;
using RichardSzalay.MockHttp;
using System.Net.Http;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;

namespace Percy.Tests
{
  public class PercyOnAutomateTest
  {
    private readonly MockDriverObject mockDriver;
    private readonly StringWriter stringWriter;
    private readonly MockHttpMessageHandler mockHttp;

    public PercyOnAutomateTest()
    {
      mockDriver = new MockDriverObject();
      mockDriver.SessionId = "session-1";
      CliWrapper.Healthcheck = () =>
      {
        return true;
      };
      stringWriter = new StringWriter();
      Console.SetOut(stringWriter);
    }

    private static String LogMessage(String label, String message, String color = "39m")
    {
      return $"[\u001b[35m{label}\u001b[{color}] {message}";
    }

    [Fact]
    public void postScreenshot()
    {
      TestHelper.UnsetEnvVariables();
      mockDriver.SetCapability(MetadataBuilder.CapabilityBuilder("Android"));
      mockDriver.setCommandExecutor("https://browserstack.com/wd/hub");
      PercyOnAutomate percy = new PercyOnAutomate(mockDriver);

      var mockHttp = new MockHttpMessageHandler();
      var obj = new
      {
        success = true,
        version = "1.0",
      };
      mockHttp.Fallback.Respond(new HttpClient());
      mockHttp.Expect(HttpMethod.Post, "http://localhost:5338/percy/automateScreenshot")
        .WithPartialContent("Screenshot 1")
        .WithPartialContent("https://browserstack.com/wd/hub")
        .WithPartialContent("session-1")
        .WithPartialContent(" \"platformName\": \"Android\"")
        .Respond("application/json", JsonConvert.SerializeObject(obj));
      CliWrapper.setHttpClient(new HttpClient(mockHttp));
      percy.Screenshot("Screenshot 1");

      mockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public void postScreenshotWithSync()
    {
      TestHelper.UnsetEnvVariables();
      mockDriver.SetCapability(MetadataBuilder.CapabilityBuilder("Android"));
      mockDriver.setCommandExecutor("https://browserstack.com/wd/hub");
      PercyOnAutomate percy = new PercyOnAutomate(mockDriver);


      var syncData = JObject.Parse("{'name': 'snapshot'}");
      var mockHttp = new MockHttpMessageHandler();
      var obj = new
      {
        success = true,
        version = "1.0",
        data = syncData
      };
      mockHttp.Fallback.Respond(new HttpClient());
      mockHttp.Expect(HttpMethod.Post, "http://localhost:5338/percy/automateScreenshot")
        .WithPartialContent("https://browserstack.com/wd/hub")
        .WithPartialContent("session-1")
        .WithPartialContent(" \"platformName\": \"Android\"")
        .Respond("application/json", JsonConvert.SerializeObject(obj));
      CliWrapper.setHttpClient(new HttpClient(mockHttp));
      Dictionary<string, object> options = new Dictionary<string, object>();
      options["sync"] = true;
      Assert.Equal(syncData, percy.Screenshot("Screenshot 1", options));

      mockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public void postScreenshotWithOptions()
    {
      TestHelper.UnsetEnvVariables();
      mockDriver.SetCapability(MetadataBuilder.CapabilityBuilder("Android"));
      mockDriver.setCommandExecutor("https://browserstack.com/wd/hub");
      PercyOnAutomate percy = new PercyOnAutomate(mockDriver);

      var mockHttp = new MockHttpMessageHandler();
      var obj = new
      {
        success = true,
        version = "1.0",
      };
      mockHttp.Fallback.Respond(new HttpClient());

      var element = new MockAppiumElement();
      var elementList = new List<object> { element };
      Dictionary<string, object> options = new Dictionary<string, object>();
      options["ignore_region_appium_elements"] = elementList;

      mockHttp.Expect(HttpMethod.Post, "http://localhost:5338/percy/automateScreenshot")
        .WithPartialContent("Screenshot 1")
        .WithPartialContent("https://browserstack.com/wd/hub")
        .WithPartialContent("session-1")
        .WithPartialContent(" \"platformName\": \"Android\"")
        .WithPartialContent("element_id")
        .Respond("application/json", JsonConvert.SerializeObject(obj));
      CliWrapper.setHttpClient(new HttpClient(mockHttp));
      percy.Screenshot("Screenshot 1", options);

      mockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public void postScreenshotHandleFailure()
    {
      TestHelper.UnsetEnvVariables();
      mockDriver.SetCapability(MetadataBuilder.CapabilityBuilder("Android"));
      mockDriver.setCommandExecutor("https://browserstack.com/wd/hub");
      PercyOnAutomate percy = new PercyOnAutomate(mockDriver);

      // Setting Expectation
      var mockHttp = new MockHttpMessageHandler();
      var obj = new
      {
        success = false,
        version = "1.0",
      };
      mockHttp.Fallback.Respond(new HttpClient());
      mockHttp.Expect(HttpMethod.Post, "http://localhost:5338/percy/automateScreenshot")
        .Respond("application/json", JsonConvert.SerializeObject(obj));
      CliWrapper.setHttpClient(new HttpClient(mockHttp));
      percy.Screenshot("Screenshot 1");

      Assert.Contains(LogMessage("percy", $"Could not take screenshot \"Screenshot 1\"\n"), stringWriter.ToString());
      mockHttp.VerifyNoOutstandingExpectation();
    }
  }
}
