using System;
using System.Globalization;
using System.IO;
using Newtonsoft.Json.Linq;
using Moq;
using Xunit;
using PercyIO.Appium;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Runtime.InteropServices.JavaScript;
using RichardSzalay.MockHttp;
using System.Net.Http;

namespace Percy.Tests
{
  public class AppAutomateTest
  {
    private readonly Mock<IPercyAppiumDriver> _androidPercyAppiumDriver = new Mock<IPercyAppiumDriver>();

    public AppAutomateTest()
    {
      _androidPercyAppiumDriver = MetadataBuilder.mockDriver("Android");
    }

    [Fact]
    public void TestSupports_WhenNotNull()
    {
      // Arrange
      String url = "http://hub-cloud.browserstack.com/wd/hub";
      _androidPercyAppiumDriver.Setup(x => x.GetHost())
        .Returns(url);
      // Act
      bool actual = AppAutomate.Supports(_androidPercyAppiumDriver.Object);
      // Assert
      Assert.True(actual);
    }

    [Fact]
    public void TestSupports_WhenNonBrowserStack()
    {
      // Arrange
      String url = "http://hub-cloud.abc.com/wd/hub";
      _androidPercyAppiumDriver.Setup(x => x.GetHost())
        .Returns(url);
      // Act
      bool actual = AppAutomate.Supports(_androidPercyAppiumDriver.Object);
      // Assert
      Assert.False(actual);
    }

    [Fact]
    public void TestSupports_WhenGetHostReturnsNull()
    {
      // Arrange — simulates Appium 8.x where GetHost() returns null
      // due to reflection failing to find remoteServerUri on derived type
      _androidPercyAppiumDriver.Setup(x => x.GetHost())
        .Returns((string)null);
      // Act
      bool actual = AppAutomate.Supports(_androidPercyAppiumDriver.Object);
      // Assert — should return false, not throw NullReferenceException
      Assert.False(actual);
    }

    [Fact]
    public void TestGetDebugUrl()
    {
      // Arrange
      string json = @"{success:'true', osVersion:'11.2', buildHash:'abc', sessionHash:'def'}";
      JObject result = JObject.Parse(json);
      string expected = "https://app-automate.browserstack.com/dashboard/v2/builds/abc/sessions/def";
      // Act
      AppAutomate appAutomate = new AppAutomate(_androidPercyAppiumDriver.Object);
      string actual = appAutomate.GetDebugUrl(result);
      // Assert
      Assert.Equal(actual, expected);
    }

    [Fact]
    public void TestScreenshot()
    {
      // Arrange
      string expected = "https://percy.io/api/v1/comparisons/redirect?snapshot[name]=test%20screenshot&tag[name]=Samsung&tag[os_name]=Android&tag[os_version]=9&tag[width]=1280&tag[height]=1420&tag[orientation]=landscape";
      Environment.SetEnvironmentVariable("PERCY_DISABLE_REMOTE_UPLOADS", "true");
      var response = @"{success:'true', osVersion:'11.2', buildHash:'abc', sessionHash:'def'}";
      _androidPercyAppiumDriver.Setup(x => x.ExecuteScript(It.IsAny<string>()))
        .Returns(response);
      AppAutomate appAutomate = new AppAutomate(_androidPercyAppiumDriver.Object);
      var options = new ScreenshotOptions();
      options.DeviceName = "Samsung";
      options.StatusBarHeight = 100;
      options.NavBarHeight = 100;
      options.Orientation = "potrait";
      options.FullScreen = false;
      options.FullPage = false;
      options.ScreenLengths = 0;

      var data = JObject.FromObject(new
      {
        snapshotname = "temp",
        status = "success"
      });

      var mockHttp = new MockHttpMessageHandler();

      // Setup a respond for the user api (including a wildcard in the URL)
      mockHttp.When("http://localhost:5338/percy/comparison")
        .Respond("application/json", "{\"success\": true, \"link\": \"" + expected + "\", \"data\": \"" + data + "\"}");  

      CliWrapper.setHttpClient(new HttpClient(mockHttp));

      // Act
      var actual = appAutomate.Screenshot("temp", options);
      // Assert
      Assert.Equal(actual, null);
      CliWrapper.resetHttpClient();
    }

    [Fact]
    public void TestScreenshot_WhenPercyScreenshotBeginReturnsNull()
    {
      // Arrange
      Environment.SetEnvironmentVariable("PERCY_DISABLE_REMOTE_UPLOADS", "true");
      _androidPercyAppiumDriver.Setup(x => x.ExecuteScript(It.IsAny<string>()))
        .Throws(new Exception());
      var options = new ScreenshotOptions();
      options.DeviceName = "Samsung";
      options.StatusBarHeight = 100;
      options.NavBarHeight = 100;
      options.Orientation = "potrait";
      options.FullScreen = false;
      options.FullPage = false;
      options.ScreenLengths = 0;

      AppAutomate appAutomate = new AppAutomate(_androidPercyAppiumDriver.Object);
      // Act
      var actual = appAutomate.Screenshot("temp", options);
      // Assert
      Assert.Equal(true, actual["success"]);
      Environment.SetEnvironmentVariable("PERCY_DISABLE_REMOTE_UPLOADS", "false");
    }

    [Fact]
    public void TestExecutePercyScreenshotBegin()
    {
      // Arrange
      var arguments = new JObject();
      var response = JObject.FromObject(new
      {
        success = true,
        deviceName = "iPhone 13",
        osVersion = "15.0",
        buildHash = "dummy_build_hash",
        sessionHash = "dummy_session_hash"
      });
      string name = "First";
      var obj = JObject.FromObject(new
      {
        action = "percyScreenshot",
        arguments = new
        {
          state = "begin",
          percyBuildId = Env.GetPercyBuildID(),
          percyBuildUrl = Env.GetPercyBuildUrl(),
          name = name
        }
      });

      _androidPercyAppiumDriver.Setup(x => x.ExecuteScript("browserstack_executor:" + obj.ToString()))
        .Returns(response.ToString());
      // Act
      var appAutomate = new AppAutomate(_androidPercyAppiumDriver.Object);
      var result = appAutomate.ExecutePercyScreenshotBegin(name);
      string actual = result.GetValue("success").ToString();
      // Assert
      Assert.Equal(actual, "True");
      _androidPercyAppiumDriver.Verify(x => x.ExecuteScript("browserstack_executor:" + obj.ToString()), Times.Once);
    }

    [Fact]
    public void TestExecutePercyScreenshotBegin_WhenThrowError()
    {
      // Arrange
      var arguments = new JObject();
      var name = "First";
      var reqObject = JObject.FromObject(new
      {
        action = "percyScreenshot",
        arguments = new
        {
          state = "begin",
          percyBuildId = Env.GetPercyBuildID(),
          percyBuildUrl = Env.GetPercyBuildUrl(),
          name = name
        }
      });
      _androidPercyAppiumDriver.Setup(x => x.ExecuteScript("browserstack_executor:" + reqObject.ToString()))
        .Throws(new Exception());
      // Act
      var appAutomate = new AppAutomate(_androidPercyAppiumDriver.Object);
      // Assert
      Assert.Throws<NullReferenceException>(() => appAutomate.ExecutePercyScreenshotBegin(name).GetValue("success").ToString());
    }

    [Fact]
    public void TestExecutePercyScreenshotEnd()
    {
      // Arrange
      Environment.SetEnvironmentVariable("PERCY_LOGLEVEL", "debug");
      var response = JObject.FromObject(new
      {
        success = true,
      });
      var name = "First";
      var percyScreenshotUrl = "";
      var reqObject = JObject.FromObject(new
      {
        action = "percyScreenshot",
        arguments = new
        {
          state = "end",
          percyScreenshotUrl = percyScreenshotUrl,
          status = "success",
          statusMessage = JValue.CreateNull(),
          name = name,
          sync = false
        }
      });

      _androidPercyAppiumDriver.Setup(x => x.ExecuteScript("browserstack_executor:" + reqObject.ToString()))
        .Returns(response.ToString());
      // Act
      var appAutomate = new AppAutomate(_androidPercyAppiumDriver.Object);
      var result = appAutomate.ExecutePercyScreenshotEnd(name, percyScreenshotUrl, false, null);
      var actual = result.GetValue("success").ToString();
      // Assert
      Assert.Equal(actual, "True");
      _androidPercyAppiumDriver.Verify(x => x.ExecuteScript("browserstack_executor:" + reqObject.ToString()), Times.Once);
    }

    [Fact]
    public void TestExecutePercyScreenshotEnd_WhenError()
    {
      // Arrange
      Environment.SetEnvironmentVariable("PERCY_LOGLEVEL", "debug");
      var response = JObject.FromObject(new
      {
        success = false,
      });
      var name = "First";
      var percyScreenshotUrl = "";
      var reqObject = JObject.FromObject(new
      {
        action = "percyScreenshot",
        arguments = new
        {
          state = "end",
          percyScreenshotUrl = percyScreenshotUrl,
          status = "failure",
          statusMessage = "some error",
          name = name,
          sync = false
        }
      });
      _androidPercyAppiumDriver.Setup(x => x.ExecuteScript("browserstack_executor:" + reqObject.ToString()))
        .Returns(response.ToString());
      // Act
      var appAutomate = new AppAutomate(_androidPercyAppiumDriver.Object);
      string actual = appAutomate.ExecutePercyScreenshotEnd(name, percyScreenshotUrl, false, "some error").GetValue("success").ToString();
      // Assert
      Assert.Equal(actual, "False");
      _androidPercyAppiumDriver.Verify(x => x.ExecuteScript("browserstack_executor:" + reqObject.ToString()), Times.Once);
    }

    [Fact]
    public void TestExecutePercyScreenshotEnd_WhenException()
    {
      // Arrange
      Environment.SetEnvironmentVariable("PERCY_LOGLEVEL", "debug");
      var name = "First";
      var percyScreenshotUrl = "";
      var reqObject = JObject.FromObject(new
      {
        action = "percyScreenshot",
        arguments = new
        {
          state = "end",
          percyScreenshotUrl = percyScreenshotUrl,
          status = "failure",
          statusMessage = "some error",
          name = name
        }
      });
      _androidPercyAppiumDriver.Setup(x => x.ExecuteScript("browserstack_executor:" + reqObject.ToString()))
        .Throws(new Exception());
      // Act
      var app = new AppAutomate(_androidPercyAppiumDriver.Object);
      // Assert
      Assert.Throws<NullReferenceException>(() => app.ExecutePercyScreenshotEnd(null, null, false, null).GetValue("success").ToString());
      _androidPercyAppiumDriver.Verify(x => x.ExecuteScript("browserstack_executor:" + reqObject.ToString()), Times.Never);
    }

    [Fact]
    public void CaptureTiles_ShouldReturnListOfTiles_WhenCalled()
    {
      // Arrange
      Environment.SetEnvironmentVariable("PERCY_DISABLE_REMOTE_UPLOADS", "false");
      _androidPercyAppiumDriver.Setup(x => x.ExecuteScript(It.IsAny<string>()))
        .Returns(JsonConvert.SerializeObject(new
        {
          success = true,
          result = JsonConvert.SerializeObject(new List<object> {
              new { sha = "abcd-1234", header_height = 50, footer_height = 30 },
              new { sha = "abce-1234", header_height = 80, footer_height = 10 }
            })
        }
        ));

      var appAutomate = new AppAutomate(_androidPercyAppiumDriver.Object);
      var metadata = new AndroidMetadata(_androidPercyAppiumDriver.Object, "Samsung Galaxy s22", 100, 200, null, null);
      appAutomate.metadata = metadata;
      var options = new ScreenshotOptions();
      options.FullScreen = false;
      options.FullPage = true;
      options.ScreenLengths = 2;
      // Act
      var result = appAutomate.CaptureTiles(options);

      // Assert
      Assert.IsType<System.Collections.Generic.List<Tile>>(result);
      Assert.Equal(2, result.Count);
      Assert.Equal("abcd", result[0].Sha);
      Assert.Equal(100, result[0].StatusBarHeight);
      Assert.Equal(200, result[0].NavBarHeight);
      Assert.Equal(50, result[0].HeaderHeight);
      Assert.Equal(30, result[0].FooterHeight);
    }

    [Fact]
    public void TestExecutePercyScreenshot()
    {
      var response = JsonConvert.SerializeObject(new
      {
        success = true,
        result = JsonConvert.SerializeObject(new List<object> {
            new { sha = "abcd-1234", header_height = 50, footer_height = 30 }
          })
      }
      );
      _androidPercyAppiumDriver.Setup(x => x.ExecuteScript(It.IsAny<string>())).Returns(response);

      var options = new ScreenshotOptions();
      options.ScreenLengths = 2;
      options.ScrollableXpath = "xapth/dummy/scrollable";
      // Act
      var appAutomate = new AppAutomate(_androidPercyAppiumDriver.Object);
      var metadata = new AndroidMetadata(_androidPercyAppiumDriver.Object, "Samsung Galaxy s22", 100, 200, null, null);
      appAutomate.metadata = metadata;
      var actual = appAutomate.ExecutePercyScreenshot(options);
      // Assert
      _androidPercyAppiumDriver.Verify(x => x.ExecuteScript(It.IsAny<string>()), Times.Once);
      Assert.Contains("abcd-1234", actual);
    }

    [Fact]
    public void TestDeviceName_WhenValueIsNull()
    {
      // Arrange
      var json = @"{deviceName:'Samsung Galaxy S22'}";
      var result = JObject.Parse(json);
      var expected = "Samsung Galaxy S22";
      // Act
      var appAutomate = new AppAutomate(_androidPercyAppiumDriver.Object);
      var actual = appAutomate.DeviceName(null, result);
      // Assert
      Assert.Equal(actual, expected);
    }

    [Fact]
    public void TestDeviceName_WhenProvidedInParams()
    {
      // Arrange
      var json = @"{deviceName:'Samsung Galaxy S22'}";
      var result = JObject.Parse(json);
      var expected = "Samsung Galaxy S21";
      // Act
      var appAutomate = new AppAutomate(_androidPercyAppiumDriver.Object);
      var actual = appAutomate.DeviceName(expected, result);
      // Assert
      Assert.Equal(actual, expected);
    }

    [Fact]
    public void TestVerifyCorrectAppiumVersion_WhenJSONFalse()
    {
      var expected = false;
      _androidPercyAppiumDriver.Setup(x => x.GetCapabilities().getValue<object>("browserstack.appium_version")).Returns("1.16.0");
      // Act
      var appAutomate = new AppAutomate(_androidPercyAppiumDriver.Object);
      var actual = appAutomate.VerifyCorrectAppiumVersion();
      // Assert
      Assert.Equal(actual, expected);
    }

    [Fact]
    public void TestVerifyCorrectAppiumVersion_WhenJSONTrue()
    {
      var expected = true;
      _androidPercyAppiumDriver.Setup(x => x.GetCapabilities().getValue<object>("browserstack.appium_version")).Returns("1.20.0");
      // Act
      var appAutomate = new AppAutomate(_androidPercyAppiumDriver.Object);
      var actual = appAutomate.VerifyCorrectAppiumVersion();
      // Assert
      Assert.Equal(actual, expected);
    }

    [Fact]
    public void TestVerifyCorrectAppiumVersion_WhenW3CFalse()
    {
      var expected = false;
      _androidPercyAppiumDriver.Setup(x => x.GetCapabilities().getValue<Dictionary<string, object>>("bstack:options")).Returns(
        new Dictionary<string, object> {
          {"appiumVersion", "1.16.0"},
        }
      );
      // Act
      var appAutomate = new AppAutomate(_androidPercyAppiumDriver.Object);
      var actual = appAutomate.VerifyCorrectAppiumVersion();
      // Assert
      Assert.Equal(actual, expected);
    }

    [Fact]
    public void TestVerifyCorrectAppiumVersion_WhenW3CTrue()
    {
      var expected = true;
      _androidPercyAppiumDriver.Setup(x => x.GetCapabilities().getValue<Dictionary<string, object>>("bstack:options")).Returns(
        new Dictionary<string, object> {
          {"appiumVersion", "1.20.0"},
        }
      );
      // Act
      var appAutomate = new AppAutomate(_androidPercyAppiumDriver.Object);
      var actual = appAutomate.VerifyCorrectAppiumVersion();
      // Assert
      Assert.Equal(actual, expected);
    }

    [Fact]
    public void TestVerifyCorrectAppiumVersion_WhenW3CWithoutAppiumVersionKey()
    {
      // `bstack:options` is always injected on a BrowserStack SDK session, but
      // `appiumVersion` is only in it when the user pins one. Indexing the missing key threw
      // KeyNotFoundException, which propagated all the way out as a silent null screenshot.
      _androidPercyAppiumDriver.Setup(x => x.GetCapabilities().getValue<Dictionary<string, object>>("bstack:options")).Returns(
        new Dictionary<string, object> {
          {"userName", "someuser"},
          {"deviceName", "Samsung Galaxy S22"},
          {"osVersion", "12.0"},
        }
      );
      var stdout = new StringWriter();
      var originalOut = Console.Out;
      Console.SetOut(stdout);
      try
      {
        // Act
        var appAutomate = new AppAutomate(_androidPercyAppiumDriver.Object);
        var actual = appAutomate.VerifyCorrectAppiumVersion();
        // Assert — unknown version must not block fullpage, and must not throw
        Assert.True(actual);
        // `true` alone cannot tell a probed key from one that threw and hit the catch-all.
        // That message is reachable only from the catch-all, so its absence proves the path.
        Assert.DoesNotContain("Unable to verify Appium version", stdout.ToString());
      }
      finally
      {
        Console.SetOut(originalOut);
      }
    }

    [Fact]
    public void TestVerifyCorrectAppiumVersion_WhenW3CAppiumVersionIsNull()
    {
      _androidPercyAppiumDriver.Setup(x => x.GetCapabilities().getValue<Dictionary<string, object>>("bstack:options")).Returns(
        new Dictionary<string, object> {
          {"appiumVersion", null},
        }
      );
      // Act
      var appAutomate = new AppAutomate(_androidPercyAppiumDriver.Object);
      var actual = appAutomate.VerifyCorrectAppiumVersion();
      // Assert
      Assert.True(actual);
    }

    [Fact]
    public void TestVerifyCorrectAppiumVersion_WhenAppiumThree()
    {
      // The gate is "Appium >= 1.19", but it was written as `major == 2` before 3.x
      // existed, so a pinned `appiumVersion: 3.1.0` silently downgraded fullpage to single page.
      _androidPercyAppiumDriver.Setup(x => x.GetCapabilities().getValue<Dictionary<string, object>>("bstack:options")).Returns(
        new Dictionary<string, object> {
          {"appiumVersion", "3.1.0"},
        }
      );
      // Act
      var appAutomate = new AppAutomate(_androidPercyAppiumDriver.Object);
      var actual = appAutomate.VerifyCorrectAppiumVersion();
      // Assert
      Assert.True(actual);
    }

    [Fact]
    public void TestVerifyCorrectAppiumVersion_WhenCapabilityLookupThrows()
    {
      // Version detection must not be able to take the screenshot down with it
      _androidPercyAppiumDriver.Setup(x => x.GetCapabilities()).Throws(new Exception("caps unavailable"));
      // Act
      var appAutomate = new AppAutomate(_androidPercyAppiumDriver.Object);
      var actual = appAutomate.VerifyCorrectAppiumVersion();
      // Assert — does not propagate, and attempts fullpage rather than silently downgrading
      Assert.True(actual);
    }

    [Fact]
    public void TestAppiumVersionCheck_WhenVersionIsEmpty()
    {
      var appAutomate = new AppAutomate(_androidPercyAppiumDriver.Object);
      // null means "cannot determine", which is distinct from "below the gate"
      Assert.Null(appAutomate.AppiumVersionCheck(null));
      Assert.Null(appAutomate.AppiumVersionCheck(""));
      Assert.Null(appAutomate.AppiumVersionCheck("   "));
    }

    [Fact]
    public void TestVerifyCorrectAppiumVersion_WarnTextNamesTheUnparsedValue()
    {
      // The fallback warnings are the only signal a user gets, so assert the text, not just
      // the boolean: reporting a parse failure as "should be >= 1.19" sends them hunting for
      // a version problem they do not have.
      var stdout = new StringWriter();
      var original = Console.Out;
      Console.SetOut(stdout);
      try
      {
        _androidPercyAppiumDriver.Setup(x => x.GetCapabilities().getValue<Dictionary<string, object>>("bstack:options")).Returns(
          new Dictionary<string, object> { { "appiumVersion", "banana" } }
        );
        var appAutomate = new AppAutomate(_androidPercyAppiumDriver.Object);
        Assert.True(appAutomate.VerifyCorrectAppiumVersion());
        var output = stdout.ToString();
        Assert.Contains("Could not parse Appium version 'banana'", output);
        // The reassurance is the whole point of not downgrading here, so assert it positively —
        // without this, deleting the post-loop message leaves the suite green.
        Assert.Contains("Attempting Fullpage Screenshot anyway", output);
        Assert.DoesNotContain("should be >= 1.19", output);
      }
      finally
      {
        Console.SetOut(original);
      }
    }

    [Fact]
    public void TestVerifyCorrectAppiumVersion_WarnTextWhenBelowGate()
    {
      var stdout = new StringWriter();
      var original = Console.Out;
      Console.SetOut(stdout);
      try
      {
        _androidPercyAppiumDriver.Setup(x => x.GetCapabilities().getValue<Dictionary<string, object>>("bstack:options")).Returns(
          new Dictionary<string, object> { { "appiumVersion", "1.18.0" } }
        );
        var appAutomate = new AppAutomate(_androidPercyAppiumDriver.Object);
        Assert.False(appAutomate.VerifyCorrectAppiumVersion());
        Assert.Contains("should be >= 1.19", stdout.ToString());
      }
      finally
      {
        Console.SetOut(original);
      }
    }

    [Fact]
    public void TestVerifyCorrectAppiumVersion_WhenValueIsLossyFloatingPoint()
    {
      // An unquoted `appiumVersion: 1.20` in browserstack.yml arrives as the double 1.2.
      // Rebuilding a string from it is lossy, so the value is not trusted at all rather than
      // being range-compared as minor 2 and wrongly downgraded.
      var stdout = new StringWriter();
      var originalOut = Console.Out;
      Console.SetOut(stdout);
      try
      {
        _androidPercyAppiumDriver.Setup(x => x.GetCapabilities().getValue<Dictionary<string, object>>("bstack:options")).Returns(
          new Dictionary<string, object> { { "appiumVersion", 1.20d } }
        );
        var appAutomate = new AppAutomate(_androidPercyAppiumDriver.Object);
        Assert.True(appAutomate.VerifyCorrectAppiumVersion());
        var output = stdout.ToString();
        Assert.Contains("Could not use Appium version capability '1.2'", output);
        Assert.Contains("Attempting Fullpage Screenshot anyway", output);
        Assert.DoesNotContain("should be >= 1.19", output);
      }
      finally
      {
        Console.SetOut(originalOut);
      }
    }

    [Fact]
    public void TestVerifyCorrectAppiumVersion_WhenValueIsUnquotedInteger()
    {
      // `appiumVersion: 2` unquoted is a long. It converts losslessly, so it must be used —
      // not rejected, which would warn on every fullpage snapshot...
      var stdout = new StringWriter();
      var originalOut = Console.Out;
      Console.SetOut(stdout);
      try
      {
        _androidPercyAppiumDriver.Setup(x => x.GetCapabilities().getValue<Dictionary<string, object>>("bstack:options")).Returns(
          new Dictionary<string, object> { { "appiumVersion", 2L } }
        );
        Assert.True(new AppAutomate(_androidPercyAppiumDriver.Object).VerifyCorrectAppiumVersion());
        // Name the strings that must not appear rather than demanding total silence, matching
        // the sibling quiet-assertions so an unrelated future log cannot fail this for the
        // wrong reason.
        var output = stdout.ToString();
        Assert.DoesNotContain("Could not use Appium version capability", output);
        Assert.DoesNotContain("Attempting Fullpage Screenshot anyway", output);
        Assert.DoesNotContain("should be >= 1.19", output);
      }
      finally
      {
        Console.SetOut(originalOut);
      }
    }

    [Fact]
    public void TestVerifyCorrectAppiumVersion_WhenValueIsUnquotedIntegerBelowGate()
    {
      // ...and rejecting it would also stop the gate being enforced for `appiumVersion: 1`.
      var stdout = new StringWriter();
      var originalOut = Console.Out;
      Console.SetOut(stdout);
      try
      {
        _androidPercyAppiumDriver.Setup(x => x.GetCapabilities().getValue<Dictionary<string, object>>("bstack:options")).Returns(
          new Dictionary<string, object> { { "appiumVersion", 1L } }
        );
        Assert.False(new AppAutomate(_androidPercyAppiumDriver.Object).VerifyCorrectAppiumVersion());
        // The integral path is the one this change started trusting, so assert it produces the
        // downgrade message and not the "cannot determine" one.
        var output = stdout.ToString();
        Assert.Contains("should be >= 1.19", output);
        Assert.DoesNotContain("Could not use Appium version capability", output);
      }
      finally
      {
        Console.SetOut(originalOut);
      }
    }

    [Fact]
    public void TestVerifyCorrectAppiumVersion_AboveGateJwpDoesNotShadowBelowGateW3CValue()
    {
      // The mirror of JwpUnparseableDoesNotShadowW3CValue: returning early on the first usable
      // value would let JWP 2.0 hide a W3C 1.16 the hub actually honours, and request a
      // fullpage capture against it with no warning at all.
      _androidPercyAppiumDriver.Setup(x => x.GetCapabilities().getValue<object>("browserstack.appium_version")).Returns("2.0");
      _androidPercyAppiumDriver.Setup(x => x.GetCapabilities().getValue<Dictionary<string, object>>("bstack:options")).Returns(
        new Dictionary<string, object> { { "appiumVersion", "1.16" } }
      );
      var stdout = new StringWriter();
      var originalOut = Console.Out;
      Console.SetOut(stdout);
      try
      {
        Assert.False(new AppAutomate(_androidPercyAppiumDriver.Object).VerifyCorrectAppiumVersion());
        Assert.Contains("Falling back to single page", stdout.ToString());
      }
      finally
      {
        Console.SetOut(originalOut);
      }
    }

    [Fact]
    public void TestVerifyCorrectAppiumVersion_StaysQuietWhenVersionSimplyNotPinned()
    {
      // The common case: bstack:options injected, no appiumVersion pinned. VerifyCorrectAppiumVersion
      // runs once per fullpage snapshot, so warning here would fire on every snapshot of every build.
      var stdout = new StringWriter();
      var originalOut = Console.Out;
      Console.SetOut(stdout);
      try
      {
        _androidPercyAppiumDriver.Setup(x => x.GetCapabilities().getValue<Dictionary<string, object>>("bstack:options")).Returns(
          new Dictionary<string, object> { { "userName", "someuser" } }
        );
        var appAutomate = new AppAutomate(_androidPercyAppiumDriver.Object);
        Assert.True(appAutomate.VerifyCorrectAppiumVersion());
        // State the intent rather than demanding total silence, so an unrelated future log
        // does not fail this for the wrong reason.
        Assert.DoesNotContain("Unable to fetch", stdout.ToString());
        Assert.DoesNotContain("should be >= 1.19", stdout.ToString());
      }
      finally
      {
        Console.SetOut(originalOut);
      }
    }

    [Fact]
    public void TestVerifyCorrectAppiumVersion_JwpUnparseableDoesNotShadowW3CValue()
    {
      // Both protocols are consulted; an unparseable JWP value must not hide a usable W3C one
      // that is genuinely below the gate.
      _androidPercyAppiumDriver.Setup(x => x.GetCapabilities().getValue<object>("browserstack.appium_version")).Returns("garbage");
      _androidPercyAppiumDriver.Setup(x => x.GetCapabilities().getValue<Dictionary<string, object>>("bstack:options")).Returns(
        new Dictionary<string, object> { { "appiumVersion", "1.16" } }
      );
      var stdout = new StringWriter();
      var originalOut = Console.Out;
      Console.SetOut(stdout);
      try
      {
        Assert.False(new AppAutomate(_androidPercyAppiumDriver.Object).VerifyCorrectAppiumVersion());
        // Must not promise a fullpage attempt and then downgrade two lines later
        Assert.DoesNotContain("Attempting Fullpage Screenshot anyway", stdout.ToString());
        Assert.Contains("Falling back to single page", stdout.ToString());
      }
      finally
      {
        Console.SetOut(originalOut);
      }
    }

    [Fact]
    public void TestVerifyCorrectAppiumVersion_NonStringJwpValueStillEnforcesGate()
    {
      // getValue<String> returns null for a non-string, so an unquoted legacy
      // `browserstack.appium_version: 1.16` used to read as "not present" and skip the gate.
      _androidPercyAppiumDriver.Setup(x => x.GetCapabilities().getValue<object>("browserstack.appium_version")).Returns(1.16d);
      var stdout = new StringWriter();
      var originalOut = Console.Out;
      Console.SetOut(stdout);
      try
      {
        // "1.16" round-trips exactly, so it is judged — refusing numbers outright would leave
        // the gate unenforced for the unquoted form.
        Assert.False(new AppAutomate(_androidPercyAppiumDriver.Object).VerifyCorrectAppiumVersion());
        var output = stdout.ToString();
        Assert.Contains("Falling back to single page", output);
        Assert.DoesNotContain("Could not use Appium version capability", output);
      }
      finally
      {
        Console.SetOut(originalOut);
      }
    }

    [Theory]
    [InlineData(1.18d)]
    [InlineData(1.17d)]
    public void TestVerifyCorrectAppiumVersion_BelowGateFloatingPointStillDowngrades(double pinned)
    {
      // Regression guard against `main`, which compared .ToString() and did downgrade these.
      _androidPercyAppiumDriver.Setup(x => x.GetCapabilities().getValue<Dictionary<string, object>>("bstack:options")).Returns(
        new Dictionary<string, object> { { "appiumVersion", pinned } }
      );
      var stdout = new StringWriter();
      var originalOut = Console.Out;
      Console.SetOut(stdout);
      try
      {
        Assert.False(new AppAutomate(_androidPercyAppiumDriver.Object).VerifyCorrectAppiumVersion());
        Assert.Contains("Falling back to single page", stdout.ToString());
      }
      finally
      {
        Console.SetOut(originalOut);
      }
    }

    [Fact]
    public void TestVerifyCorrectAppiumVersion_AboveGateFloatingPointIsSilent()
    {
      // Runs once per fullpage snapshot, so a value read exactly must not warn every time.
      _androidPercyAppiumDriver.Setup(x => x.GetCapabilities().getValue<Dictionary<string, object>>("bstack:options")).Returns(
        new Dictionary<string, object> { { "appiumVersion", 1.22d } }
      );
      var stdout = new StringWriter();
      var originalOut = Console.Out;
      Console.SetOut(stdout);
      try
      {
        Assert.True(new AppAutomate(_androidPercyAppiumDriver.Object).VerifyCorrectAppiumVersion());
        var output = stdout.ToString();
        Assert.DoesNotContain("Could not use Appium version capability", output);
        Assert.DoesNotContain("Attempting Fullpage Screenshot anyway", output);
        Assert.DoesNotContain("should be >= 1.19", output);
      }
      finally
      {
        Console.SetOut(originalOut);
      }
    }

    [Fact]
    public void TestVerifyCorrectAppiumVersion_DecimalKeepsItsTrailingZero()
    {
      // The 1.20 ambiguity is a binary-float artefact; `decimal` keeps the trailing zero.
      _androidPercyAppiumDriver.Setup(x => x.GetCapabilities().getValue<Dictionary<string, object>>("bstack:options")).Returns(
        new Dictionary<string, object> { { "appiumVersion", 1.20m } }
      );
      var stdout = new StringWriter();
      var originalOut = Console.Out;
      Console.SetOut(stdout);
      try
      {
        Assert.True(new AppAutomate(_androidPercyAppiumDriver.Object).VerifyCorrectAppiumVersion());
        Assert.DoesNotContain("Could not use Appium version capability", stdout.ToString());
      }
      finally
      {
        Console.SetOut(originalOut);
      }
    }

    [Fact]
    public void TestVerifyCorrectAppiumVersion_UndeterminedValueAlwaysStatesTheConsequence()
    {
      // An above-gate value on one protocol must not suppress the reassurance for an
      // undetermined value on the other, leaving a bare "Could not use..." with no
      // stated outcome.
      _androidPercyAppiumDriver.Setup(x => x.GetCapabilities().getValue<object>("browserstack.appium_version")).Returns("2.0");
      _androidPercyAppiumDriver.Setup(x => x.GetCapabilities().getValue<Dictionary<string, object>>("bstack:options")).Returns(
        new Dictionary<string, object> { { "appiumVersion", 1.20d } }
      );
      var stdout = new StringWriter();
      var originalOut = Console.Out;
      Console.SetOut(stdout);
      try
      {
        Assert.True(new AppAutomate(_androidPercyAppiumDriver.Object).VerifyCorrectAppiumVersion());
        var output = stdout.ToString();
        // The one unrecoverable shape: 1.20 renders "1.2", and minor 2 vs 20 straddles the gate.
        Assert.Contains("Could not use Appium version capability '1.2'", output);
        Assert.Contains("Attempting Fullpage Screenshot anyway", output);
      }
      finally
      {
        Console.SetOut(originalOut);
      }
    }

    [Fact]
    public void TestExecutePercyScreenshot_SurfacesHubRefusalMessage()
    {
      // A hub that will not service the request replies {"success": false, "message": ...} with
      // no "result" key. That has to reach the user as the hub's own words — indexing "result"
      // blindly produced a bare NullReferenceException and discarded them.
      _androidPercyAppiumDriver.Setup(x => x.ExecuteScript(It.IsAny<string>()))
        .Returns("{\"success\": false, \"message\": \"fullpage not supported on this device\"}");

      var options = new ScreenshotOptions { FullPage = true, ScreenLengths = 4 };
      var appAutomate = new AppAutomate(_androidPercyAppiumDriver.Object);
      appAutomate.metadata = new AndroidMetadata(_androidPercyAppiumDriver.Object, "Samsung Galaxy s22", 100, 200, null, null);

      var ex = Assert.Throws<Exception>(() => appAutomate.ExecutePercyScreenshot(options));
      Assert.Contains("fullpage not supported on this device", ex.Message);
      Assert.Contains("was refused by BrowserStack", ex.Message);
      Assert.IsNotType<NullReferenceException>(ex);
    }

    [Fact]
    public void TestExecutePercyScreenshot_DoesNotClaimRefusalWhenSuccessIsTrue()
    {
      // A malformed success — success:true with no "result" — is a hub-side bug, not a refusal.
      // Calling it a refusal would send users hunting a permission problem they do not have.
      _androidPercyAppiumDriver.Setup(x => x.ExecuteScript(It.IsAny<string>()))
        .Returns("{\"success\": true}");

      var options = new ScreenshotOptions { FullPage = true, ScreenLengths = 4 };
      var appAutomate = new AppAutomate(_androidPercyAppiumDriver.Object);
      appAutomate.metadata = new AndroidMetadata(_androidPercyAppiumDriver.Object, "Samsung Galaxy s22", 100, 200, null, null);

      var ex = Assert.Throws<Exception>(() => appAutomate.ExecutePercyScreenshot(options));
      Assert.Contains("returned no result", ex.Message);
      Assert.DoesNotContain("refused by BrowserStack", ex.Message);
    }

    [Fact]
    public void TestAppiumVersionCheck_AcrossMajors()
    {
      var appAutomate = new AppAutomate(_androidPercyAppiumDriver.Object);
      // Below the 1.19 floor
      Assert.False(appAutomate.AppiumVersionCheck("1.18.0"));
      Assert.Null(appAutomate.AppiumVersionCheck("not-a-version"));
      // A present-but-unparseable minor is "cannot determine", not "below the gate"
      Assert.Null(appAutomate.AppiumVersionCheck("1.19-beta"));
      Assert.Null(appAutomate.AppiumVersionCheck("1.x"));
      // The floor and everything above it, including majors that did not exist when
      // the check was written
      Assert.True(appAutomate.AppiumVersionCheck("1.19.0"));
      Assert.True(appAutomate.AppiumVersionCheck("2.0.0"));
      Assert.True(appAutomate.AppiumVersionCheck("3.1.0"));
      Assert.True(appAutomate.AppiumVersionCheck("4.0.0"));
    }

    [Fact]
    public void TestVerifyCorrectAppiumVersion_WhenMajorOnlyVersion()
    {
      // "appiumVersion: 2" is a legal pin — a missing minor must not throw IndexOutOfRange
      _androidPercyAppiumDriver.Setup(x => x.GetCapabilities().getValue<Dictionary<string, object>>("bstack:options")).Returns(
        new Dictionary<string, object> {
          {"appiumVersion", "2"},
        }
      );
      var stdout = new StringWriter();
      var originalOut = Console.Out;
      Console.SetOut(stdout);
      try
      {
        // Act
        var appAutomate = new AppAutomate(_androidPercyAppiumDriver.Object);
        var actual = appAutomate.VerifyCorrectAppiumVersion();
        // Assert
        Assert.True(actual);
        // "2" is above the gate, so a caught IndexOutOfRange also yields `true`.
        Assert.DoesNotContain("Unable to verify Appium version", stdout.ToString());
      }
      finally
      {
        Console.SetOut(originalOut);
      }
    }

    [Fact]
    public void TestVerifyCorrectAppiumVersion_WhenVersionIsUnparseable()
    {
      _androidPercyAppiumDriver.Setup(x => x.GetCapabilities().getValue<object>("browserstack.appium_version")).Returns("not-a-version");
      // Act
      var appAutomate = new AppAutomate(_androidPercyAppiumDriver.Object);
      var actual = appAutomate.VerifyCorrectAppiumVersion();
      // Assert — unknown version attempts fullpage, and the warning names the parse failure
      // rather than claiming the version is below the gate
      Assert.True(actual);
    }

    [Fact]
    public void TestExecutePercyScreenshot_FullPageWhenW3CWithoutAppiumVersionKey()
    {
      // End-to-end regression: FullPage=true + ScreenLengths>=2 is the only path that
      // evaluates VerifyCorrectAppiumVersion(), which is why FullPage=false kept working while
      // fullpage silently produced no snapshot at all.
      Environment.SetEnvironmentVariable("PERCY_DISABLE_REMOTE_UPLOADS", "false");
      // Real capability lookup (not a stubbed getValue) so the missing-key path is genuinely
      // exercised — `bstack:options` present, `appiumVersion` absent, as the BrowserStack SDK
      // sends it whenever the user has not pinned a version.
      var caps = new PercyAppiumCapabilities();
      var capsDict = MetadataBuilder.CapabilityBuilder("Android");
      capsDict.Add("bstack:options", new Dictionary<string, object> {
        {"userName", "someuser"},
        {"deviceName", "Samsung Galaxy S22"},
      });
      caps.SetCapability(capsDict);
      _androidPercyAppiumDriver.Setup(x => x.GetCapabilities()).Returns(caps);
      var response = JsonConvert.SerializeObject(new
      {
        success = true,
        result = JsonConvert.SerializeObject(new List<object> {
            new { sha = "abcd-1234", header_height = 50, footer_height = 30 }
          })
      });
      string captured = null;
      _androidPercyAppiumDriver.Setup(x => x.ExecuteScript(It.IsAny<string>()))
        .Callback<string>(s => captured = s)
        .Returns(response);
      var appAutomate = new AppAutomate(_androidPercyAppiumDriver.Object);
      appAutomate.metadata = new AndroidMetadata(_androidPercyAppiumDriver.Object, "Samsung Galaxy s22", 100, 200, null, null);
      var options = new ScreenshotOptions();
      options.FullPage = true;
      options.ScreenLengths = 4;
      var stdout = new StringWriter();
      var originalOut = Console.Out;
      Console.SetOut(stdout);
      string result;
      try
      {
        // Act
        result = appAutomate.ExecutePercyScreenshot(options);
        // The catch-all also requests fullpage, so the assertions below pass either way.
        Assert.DoesNotContain("Unable to verify Appium version", stdout.ToString());
      }
      finally
      {
        Console.SetOut(originalOut);
      }
      // Assert — the executor is actually reached, and asked for a fullpage capture
      Assert.NotNull(result);
      Assert.Contains("abcd-1234", result);
      Assert.Contains("fullpage", captured);
      Assert.DoesNotContain("singlepage", captured);
    }

    [Fact]
    public void TestExecutePercyScreenshotBegin_WhenSessionNotMarked()
    {
      // Arrange — first begin returns success=false, flipping markedPercySession to false
      var name = "First";
      var failureResponse = JObject.FromObject(new { success = false });
      _androidPercyAppiumDriver.Setup(x => x.ExecuteScript(It.IsAny<string>()))
        .Returns(failureResponse.ToString());
      var appAutomate = new AppAutomate(_androidPercyAppiumDriver.Object);
      // First call marks the session as not a Percy session
      appAutomate.ExecutePercyScreenshotBegin(name);
      // Act — second call should skip the executor block and return null (covers line 60)
      var result = appAutomate.ExecutePercyScreenshotBegin(name);
      // Assert
      Assert.Null(result);
      _androidPercyAppiumDriver.Verify(x => x.ExecuteScript(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public void TestExecutePercyScreenshotEnd_WhenSessionNotMarked()
    {
      // Arrange — begin returns success=false so markedPercySession becomes false
      Environment.SetEnvironmentVariable("PERCY_LOGLEVEL", "debug");
      var name = "First";
      var failureResponse = JObject.FromObject(new { success = false });
      _androidPercyAppiumDriver.Setup(x => x.ExecuteScript(It.IsAny<string>()))
        .Returns(failureResponse.ToString());
      var appAutomate = new AppAutomate(_androidPercyAppiumDriver.Object);
      appAutomate.ExecutePercyScreenshotBegin(name);
      // Act — end should skip the executor block and return null (covers line 99)
      var result = appAutomate.ExecutePercyScreenshotEnd(name, "", false, null);
      // Assert
      Assert.Null(result);
      _androidPercyAppiumDriver.Verify(x => x.ExecuteScript(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public void TestScreenshot_WhenBaseScreenshotThrows()
    {
      // Arrange — begin succeeds (response has success + metadata fields) but the
      // executor response has no "result" key, so ExecutePercyScreenshot (invoked from
      // CaptureTiles inside base.Screenshot) throws, hitting the catch block (lines 125-128).
      Environment.SetEnvironmentVariable("PERCY_DISABLE_REMOTE_UPLOADS", "false");
      Environment.SetEnvironmentVariable("PERCY_LOGLEVEL", "debug");
      var response = @"{success:'true', osVersion:'11.2', buildHash:'abc', sessionHash:'def', deviceName:'Samsung'}";
      _androidPercyAppiumDriver.Setup(x => x.ExecuteScript(It.IsAny<string>()))
        .Returns(response);
      var appAutomate = new AppAutomate(_androidPercyAppiumDriver.Object);
      var options = new ScreenshotOptions();
      options.DeviceName = "Samsung";
      options.StatusBarHeight = 100;
      options.NavBarHeight = 100;
      options.Orientation = "potrait";
      options.FullScreen = false;
      options.FullPage = true;
      options.ScreenLengths = 2;
      // Act + Assert — the exception is re-thrown after being captured for the end event
      Assert.ThrowsAny<Exception>(() => appAutomate.Screenshot("temp", options));
      Environment.SetEnvironmentVariable("PERCY_DISABLE_REMOTE_UPLOADS", "false");
    }

    [Fact]
    public void TestCaptureTiles_WhenDisableRemoteUploadsAndFullPage()
    {
      // Arrange — remote uploads disabled + FullPage true logs a warning and falls back
      // to base.CaptureTiles (covers lines 142-144).
      Environment.SetEnvironmentVariable("PERCY_DISABLE_REMOTE_UPLOADS", "true");
      Environment.SetEnvironmentVariable("PERCY_LOGLEVEL", "debug");
      _androidPercyAppiumDriver.Setup(x => x.GetScreenshot())
        .Returns("iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mNk+A8AAQUBAScY42YAAAAASUVORK5CYII=");
      _androidPercyAppiumDriver.Setup(x => x.GetHost())
        .Returns("http://hub-cloud.browserstack.com/wd/hub");
      var appAutomate = new AppAutomate(_androidPercyAppiumDriver.Object);
      var metadata = new AndroidMetadata(_androidPercyAppiumDriver.Object, "Samsung Galaxy s22", 100, 200, null, null);
      appAutomate.metadata = metadata;
      var options = new ScreenshotOptions();
      options.FullScreen = false;
      options.FullPage = true;
      options.ScreenLengths = 2;
      // Act
      var result = appAutomate.CaptureTiles(options);
      // Assert — base.CaptureTiles returns a single tile
      Assert.IsType<System.Collections.Generic.List<Tile>>(result);
      Assert.Single(result);
      Environment.SetEnvironmentVariable("PERCY_DISABLE_REMOTE_UPLOADS", "false");
    }

    [Fact]
    public void TestCaptureTiles_WhenInvalidJsonThrows()
    {
      // Arrange — ExecutePercyScreenshot returns a non-array "result", so JArray.Parse
      // throws and is wrapped into a new Exception (covers lines 156-159).
      Environment.SetEnvironmentVariable("PERCY_DISABLE_REMOTE_UPLOADS", "false");
      _androidPercyAppiumDriver.Setup(x => x.ExecuteScript(It.IsAny<string>()))
        .Returns(JsonConvert.SerializeObject(new
        {
          success = true,
          result = "not-a-json-array"
        }));
      var appAutomate = new AppAutomate(_androidPercyAppiumDriver.Object);
      var metadata = new AndroidMetadata(_androidPercyAppiumDriver.Object, "Samsung Galaxy s22", 100, 200, null, null);
      appAutomate.metadata = metadata;
      var options = new ScreenshotOptions();
      options.FullScreen = false;
      options.FullPage = true;
      options.ScreenLengths = 2;
      // Act + Assert
      var ex = Assert.Throws<Exception>(() => appAutomate.CaptureTiles(options));
      // The message must identify the stage that failed, not just say "Error"
      Assert.Contains("percyScreenshot executor", ex.Message);
      Assert.NotNull(ex.InnerException);
    }

    [Fact]
    public void TestExecutePercyScreenshot_WhenPercyDevEnabled()
    {
      // Arrange — PERCY_ENABLE_DEV switches projectId to "percy-dev" (covers lines 182-184)
      Environment.SetEnvironmentVariable("PERCY_ENABLE_DEV", "true");
      var response = JsonConvert.SerializeObject(new
      {
        success = true,
        result = JsonConvert.SerializeObject(new List<object> {
            new { sha = "abcd-1234", header_height = 50, footer_height = 30 }
          })
      });
      string captured = null;
      _androidPercyAppiumDriver.Setup(x => x.ExecuteScript(It.IsAny<string>()))
        .Callback<string>(s => captured = s)
        .Returns(response);
      var options = new ScreenshotOptions();
      options.ScreenLengths = 2;
      options.ScrollableXpath = "xapth/dummy/scrollable";
      // Act
      var appAutomate = new AppAutomate(_androidPercyAppiumDriver.Object);
      var metadata = new AndroidMetadata(_androidPercyAppiumDriver.Object, "Samsung Galaxy s22", 100, 200, null, null);
      appAutomate.metadata = metadata;
      var actual = appAutomate.ExecutePercyScreenshot(options);
      // Assert — the request payload carries the dev project id
      Assert.Contains("percy-dev", captured);
      Assert.Contains("abcd-1234", actual);
      Environment.SetEnvironmentVariable("PERCY_ENABLE_DEV", "false");
    }
  }
}
