using System;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Appium.iOS;

namespace PercyIO.Appium
{
  public class AppPercy
  {
    public static readonly bool DEBUG = Environment.GetEnvironmentVariable("PERCY_LOGLEVEL") == "debug";
    private Boolean isPercyEnabled;
    public static Boolean ignoreErrors = true;
    private PercyOptions percyOptions;
    private IPercyAppiumDriver percyAppiumDriver;
    private String sessionId;
    public static Cache<string, object> cache = new Cache<string, object>();

    public AppPercy(AndroidDriver<AndroidElement> driver)
    {
      this.percyAppiumDriver = new PercyAppiumDriver(driver);
      setValues(this.percyAppiumDriver);
    }

    public AppPercy(IOSDriver<IOSElement> driver)
    {
      this.percyAppiumDriver = new PercyAppiumDriver(driver);
      setValues(this.percyAppiumDriver);
    }

    internal AppPercy(IPercyAppiumDriver driver)
    {
      this.percyAppiumDriver = driver;
      setValues(this.percyAppiumDriver);
    }

    internal void setValues(IPercyAppiumDriver percyAppiumDriver)
    {
      this.percyOptions = new PercyOptions(percyAppiumDriver);
      this.isPercyEnabled = CliWrapper.Healthcheck();
      this.sessionId = percyAppiumDriver.sessionId();
    }

    public void Screenshot(String name, ScreenshotOptions? options = null, Boolean fullScreen = false)
    {
      if (options == null) {
        options = new ScreenshotOptions();
      }
      options.FullScreen = fullScreen;
      if (!isPercyEnabled || !percyOptions.PercyEnabled())
      {
        return;
      }
      percyOptions.SetPercyIgnoreErrors();
      try
      {
        GenericProvider provider;
        provider = ProviderResolver.ResolveProvider(percyAppiumDriver);
        provider.Screenshot(
          name,
          options.DeviceName,
          options.StatusBarHeight,
          options.NavBarHeight,
          options.Orientation,
          fullScreen,
          options.FullPage,
          options.ScreenLengths
        );
      }
      catch (Exception e)
      {
        Log("Error taking screenshot " + name);
        if (!ignoreErrors)
        {
          throw new Exception("Error taking screenshot " + name, e);
        }
      }
    }

    ~AppPercy()
    {
      AppPercy.cache.Remove("percyOptions_" + sessionId);
      AppPercy.cache.Remove("viewportRect_" + sessionId);
    }

    internal static void Log(String message, String logLevel = "info")
    {
      if (logLevel == "debug" && DEBUG)
      {
        string label = "percy:dotnet";
        Console.WriteLine($"[\u001b[35m{label}\u001b[39m] {message}");
      }
      else if (logLevel == "info")
      {
        string label = "percy";
        Console.WriteLine($"[\u001b[35m{label}\u001b[39m] {message}");
      }
    }
  }
}
