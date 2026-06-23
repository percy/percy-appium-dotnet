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

    [Fact]
    public void postScreenshotWithConsiderRegionElements()
    {
      // Covers the consider_region_appium_elements branch (lines 44-52)
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
      options["consider_region_appium_elements"] = elementList;

      mockHttp.Expect(HttpMethod.Post, "http://localhost:5338/percy/automateScreenshot")
        .WithPartialContent("Screenshot 1")
        .WithPartialContent("https://browserstack.com/wd/hub")
        .WithPartialContent("session-1")
        .WithPartialContent(" \"platformName\": \"Android\"")
        .WithPartialContent("consider_region_elements")
        .WithPartialContent("element_id")
        .Respond("application/json", JsonConvert.SerializeObject(obj));
      CliWrapper.setHttpClient(new HttpClient(mockHttp));
      percy.Screenshot("Screenshot 1", options);

      mockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public void postScreenshotWithIgnoreAndConsiderRegionElements()
    {
      // Covers both ignore (34-42) and consider (44-52) branches together
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

      var elementList = new List<object> { new MockAppiumElement() };
      Dictionary<string, object> options = new Dictionary<string, object>();
      options["ignore_region_appium_elements"] = elementList;
      options["consider_region_appium_elements"] = elementList;

      mockHttp.Expect(HttpMethod.Post, "http://localhost:5338/percy/automateScreenshot")
        .WithPartialContent("ignore_region_elements")
        .WithPartialContent("consider_region_elements")
        .WithPartialContent("element_id")
        .Respond("application/json", JsonConvert.SerializeObject(obj));
      CliWrapper.setHttpClient(new HttpClient(mockHttp));
      percy.Screenshot("Screenshot 1", options);

      mockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public void postScreenshotCatchesException()
    {
      // Covers the catch block (lines 58-62): a non-element object in the ignore list
      // makes GetElementIds -> PercyAppiumElement reflection throw, which the Screenshot
      // try/catch swallows and returns null.
      TestHelper.UnsetEnvVariables();
      mockDriver.SetCapability(MetadataBuilder.CapabilityBuilder("Android"));
      mockDriver.setCommandExecutor("https://browserstack.com/wd/hub");
      PercyOnAutomate percy = new PercyOnAutomate(mockDriver);

      // A non-element object causes GetElementIds -> PercyAppiumElement reflection to throw,
      // which is caught by the Screenshot catch block.
      Dictionary<string, object> options = new Dictionary<string, object>();
      options["ignore_region_appium_elements"] = new List<object> { new object() };

      var result = percy.Screenshot("Screenshot 1", options);

      Assert.Null(result);
      Assert.Contains(LogMessage("percy", $"Could not take Percy Screenshot \"Screenshot 1\"\n"), stringWriter.ToString());
    }

    [Fact]
    public void screenshotWithScreenshotOptionsThrows()
    {
      // Covers the second Screenshot overload (lines 65-66)
      TestHelper.UnsetEnvVariables();
      mockDriver.SetCapability(MetadataBuilder.CapabilityBuilder("Android"));
      mockDriver.setCommandExecutor("https://browserstack.com/wd/hub");
      PercyOnAutomate percy = new PercyOnAutomate(mockDriver);

      var ex = Assert.Throws<Exception>(() => percy.Screenshot("Screenshot 1", new ScreenshotOptions(), false));
      Assert.Equal("Options need to be passed using Dictionary for: Screenshot 1", ex.Message);
    }
  }
}
