using System;
using System.Collections.Generic;
using PercyIO.Appium;
using Xunit;

namespace Percy.Tests
{
  public class PercyAppiumDriverTest
  {
    public PercyAppiumDriverTest()
    {
      AppPercy.cache.Clear();
      TestHelper.UnsetEnvVariables();
    }

    // ---- Test doubles -------------------------------------------------------
    // These plain objects are passed (as System.Object) to the PercyAppiumDriver
    // constructor. PercyAppiumDriver reflects over them via ReflectionUtils, so the
    // member shapes below mirror what the real Appium driver exposes.

    private class WindowSize
    {
      public int Width { get; set; } = 360;
      public int Height { get; set; } = 640;
    }

    private class Window
    {
      public WindowSize Size { get; } = new WindowSize();
    }

    private class Manager
    {
      public Window Window { get; } = new Window();
    }

    // ToString() returns null on purpose so PercyAppiumDriver.sessionId() yields
    // null and getSessionId() falls into the dynamic-driver fallback branch.
    private class NullStringSessionId
    {
      public override string ToString() => null!;
    }

    // Full-featured fake driver covering the happy-path reflective members.
    private class FakeDriver
    {
      public List<object> ExecuteScriptArgs = new List<object>();
      public string? OrientationValue;
      public object SessionIdValue = "session-123";
      public string ScriptReturn = "script-result";

      public string? Orientation => OrientationValue;
      public object SessionId => SessionIdValue;

      public Manager Manage() => new Manager();

      public object ExecuteScript(string script, object[] args)
      {
        ExecuteScriptArgs.Add(script);
        ExecuteScriptArgs.Add(args);
        return ScriptReturn;
      }

      public object FindElement(string by, string value)
      {
        return new MockAppiumElement();
      }
    }

    // Driver whose FindElement(string,string) returns null -> wrapper returns null.
    private class NullElementDriver
    {
      public object FindElement(string by, string value) => null!;
    }

    // Driver that has no FindElement(string,string) overload at all
    // (only a different signature), exercising the method == null branch.
    private class NoFindElementDriver
    {
      public object FindElement(string only) => null!;
    }

    // Driver whose FindElement(string,string) throws, exercising the catch branch.
    private class ThrowingFindElementDriver
    {
      public object FindElement(string by, string value)
      {
        throw new InvalidOperationException("boom");
      }
    }

    // Driver without an ExecuteScript(string, object[]) overload -> private
    // ExecuteScript returns null.
    private class NoExecuteScriptDriver
    {
    }

    // ---- CommandExecutor shapes for GetHost ---------------------------------

    private class InternalExecutorHolder
    {
      private object remoteServerUri;
      public void SetUrl(string url) => remoteServerUri = new Uri(url);
    }

    // V5-style executor that exposes `internalExecutor` (and no RealExecutor),
    // forcing GetHostV5 down the internalExecutor branch.
    private class InternalExecutorCommandExecutor
    {
      private object internalExecutor;
      public InternalExecutorCommandExecutor(string url)
      {
        var holder = new InternalExecutorHolder();
        holder.SetUrl(url);
        internalExecutor = holder;
      }
    }

    private class InternalExecutorDriver
    {
      private object commandExecutor;
      public InternalExecutorDriver(string url)
      {
        commandExecutor = new InternalExecutorCommandExecutor(url);
      }
      public object CommandExecutor => commandExecutor;
    }

    // ---- Tests --------------------------------------------------------------

    // Lines 22-25
    [Fact]
    public void Orientation_ReturnsDriverOrientation()
    {
      var raw = new FakeDriver { OrientationValue = "LANDSCAPE" };
      var driver = new PercyAppiumDriver(raw);

      Assert.Equal("LANDSCAPE", driver.Orientation());
    }

    // Lines 59-65
    [Fact]
    public void DownscaledWidth_ReturnsWindowSizeWidth()
    {
      var raw = new FakeDriver();
      var driver = new PercyAppiumDriver(raw);

      Assert.Equal(360, driver.DownscaledWidth());
    }

    // Lines 67-70 (public) + 197-203 (private ExecuteScript happy path)
    [Fact]
    public void ExecuteDriverScript_DelegatesToReflectedExecuteScript()
    {
      var raw = new FakeDriver { ScriptReturn = "driver-script-out" };
      var driver = new PercyAppiumDriver(raw);

      var result = driver.ExecuteDriverScript("myScript");

      Assert.Equal("driver-script-out", result);
      Assert.Equal("myScript", raw.ExecuteScriptArgs[0]);
    }

    // Lines 53-57 (public ExecuteScript) + 202-203
    [Fact]
    public void ExecuteScript_ReturnsStringResult()
    {
      var raw = new FakeDriver { ScriptReturn = "abc" };
      var driver = new PercyAppiumDriver(raw);

      Assert.Equal("abc", driver.ExecuteScript("script"));
    }

    // Lines 197-205: private ExecuteScript returns null when no overload exists.
    [Fact]
    public void ExecuteDriverScript_ReturnsNull_WhenNoExecuteScriptOverload()
    {
      var raw = new NoExecuteScriptDriver();
      var driver = new PercyAppiumDriver(raw);

      Assert.Null(driver.ExecuteDriverScript("script"));
    }

    // Lines 72-80: getSessionId happy path (sessionId not null) returns it directly.
    [Fact]
    public void GetSessionId_ReturnsSessionId_WhenNotNull()
    {
      var raw = new FakeDriver { SessionIdValue = "session-xyz" };
      var driver = new PercyAppiumDriver(raw);

      Assert.Equal("session-xyz", driver.getSessionId());
    }

    // Lines 93-101: FindElementsByAccessibilityId returns a wrapped element.
    [Fact]
    public void FindElementsByAccessibilityId_ReturnsWrappedElement()
    {
      var raw = new FakeDriver();
      var driver = new PercyAppiumDriver(raw);

      var element = driver.FindElementsByAccessibilityId("some-id");

      Assert.NotNull(element);
      Assert.IsType<PercyAppiumElement>(element);
      Assert.Equal("element_id", element.id);
    }

    // Lines 93-100 + 110: FindElementsByAccessibilityId returns null when no element.
    [Fact]
    public void FindElementsByAccessibilityId_ReturnsNull_WhenElementMissing()
    {
      var raw = new NullElementDriver();
      var driver = new PercyAppiumDriver(raw);

      Assert.Null(driver.FindElementsByAccessibilityId("missing"));
    }

    // Lines 103-111: FindElementByXPath returns a wrapped element.
    [Fact]
    public void FindElementByXPath_ReturnsWrappedElement()
    {
      var raw = new FakeDriver();
      var driver = new PercyAppiumDriver(raw);

      var element = driver.FindElementByXPath("//xpath");

      Assert.NotNull(element);
      Assert.IsType<PercyAppiumElement>(element);
      Assert.Equal("element_id", element.id);
    }

    // Lines 103-106 + 110: FindElementByXPath returns null when no element.
    [Fact]
    public void FindElementByXPath_ReturnsNull_WhenElementMissing()
    {
      var raw = new NullElementDriver();
      var driver = new PercyAppiumDriver(raw);

      Assert.Null(driver.FindElementByXPath("//missing"));
    }

    // Lines 174-186 + 193: private FindElement logs and returns null when the
    // driver lacks a matching FindElement(string,string) overload.
    [Fact]
    public void FindElement_ReturnsNull_WhenMethodMissing()
    {
      var raw = new NoFindElementDriver();
      var driver = new PercyAppiumDriver(raw);

      Assert.Null(driver.FindElementByXPath("//x"));
      Assert.Null(driver.FindElementsByAccessibilityId("x"));
    }

    // Lines 174-181 + 188-193: private FindElement catch branch returns null
    // when invoking FindElement throws.
    [Fact]
    public void FindElement_ReturnsNull_WhenInvocationThrows()
    {
      var raw = new ThrowingFindElementDriver();
      var driver = new PercyAppiumDriver(raw);

      Assert.Null(driver.FindElementByXPath("//x"));
      Assert.Null(driver.FindElementsByAccessibilityId("x"));
    }

    // Lines 143-170 (GetHost) including 159-162: internalExecutor branch of GetHostV5.
    [Fact]
    public void GetHost_ResolvesUrl_ViaInternalExecutorBranch()
    {
      var raw = new InternalExecutorDriver("https://hub-cloud.browserstack.com/wd/hub");
      var driver = new PercyAppiumDriver(raw);

      var host = driver.GetHost();

      Assert.Equal("https://hub-cloud.browserstack.com/wd/hub", host);
    }
  }
}
