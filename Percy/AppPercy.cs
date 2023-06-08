using System;

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

    public AppPercy(Object driver)
    {
      if(!Utils.isValidDriverObject(driver))
      {
        Log("Driver object is not the type of AndroidDriver or IOSDriver. The percy command may break.", "warn");
      }
      this.percyAppiumDriver = new PercyAppiumDriver(driver);
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
      if (options == null)
      {
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
          options
        );
      }
      catch (Exception e)
      {
        if (e is PercyException)
        {
          Log("The method is not valid for current driver. Please contact us.", "warn");
        }
        Log("Error taking screenshot " + name);
        if (true)
        {
          throw new Exception("Error taking screenshot " + name, e);
        }
      }
    }

    ~AppPercy()
    {
      AppPercy.cache.Remove("percyOptions_" + sessionId);
      AppPercy.cache.Remove("viewportRect_" + sessionId);
      AppPercy.cache.Remove("session_" + sessionId);
      AppPercy.cache.Remove("caps_" + sessionId);
    }

    internal static void Log(String message, String logLevel = "info")
    {
      if (logLevel == "debug" && DEBUG)
      {
        string label = "percy:dotnet";
        LogMessage(message, label, "91m");
      }
      else if (logLevel == "info")
      {
        string label = "percy";
        LogMessage(message, label);
      }
      else if (logLevel == "warn")
      {
        string label = "percy:dotnet";
        LogMessage(message, label, "93m");
      }
    }

    private static void LogMessage(String message, String label, String color = "39m")
    {
      Console.WriteLine($"[\u001b[35m{label}\u001b[{color}] {message}");
    }
  }
}
