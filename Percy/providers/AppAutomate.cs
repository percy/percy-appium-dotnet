using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace PercyIO.Appium
{
  internal class AppAutomate : GenericProvider
  {
    private Boolean markedPercySession = true;
    private IPercyAppiumDriver percyAppiumDriver;
    private string debugUrl;

    internal AppAutomate(IPercyAppiumDriver percyAppiumDriver) : base(percyAppiumDriver)
    {
      this.percyAppiumDriver = percyAppiumDriver;
    }

    internal static bool Supports(IPercyAppiumDriver percyAppiumDriver)
    {
      string remoteAddress = percyAppiumDriver.GetHost();
      if (remoteAddress.Contains(Environment.GetEnvironmentVariable("AA_DOMAIN") != null
          ? Environment.GetEnvironmentVariable("AA_DOMAIN") : "browserstack"))
      {
        return true;
      }
      return false;
    }

    internal void SetDebugUrl(JObject result)
    {
      var buildHash = result.GetValue("buildHash").ToString();
      var sessionHash = result.GetValue("sessionHash").ToString();
      this.debugUrl = "https://app-automate.browserstack.com/dashboard/v2/builds/" + buildHash + "/sessions/" + sessionHash;
    }

    internal string GetDebugUrl()
    {
      return this.debugUrl;
    }

    internal JObject ExecutePercyScreenshotBegin(String name)
    {
      try
      {
        if (markedPercySession)
        {
          var obj = JObject.FromObject(new
          {
            action = "percyScreenshot",
            arguments = new
            {
              state = "begin",
              percyBuildId = Environment.GetEnvironmentVariable("PERCY_BUILD_ID"),
              percyBuildUrl = Environment.GetEnvironmentVariable("PERCY_BUILD_URL"),
              name = name
            }
          });
          var resultString = percyAppiumDriver.ExecuteScript("browserstack_executor:" + obj.ToString());
          var result = JObject.Parse(resultString);
          markedPercySession = (result.GetValue("success").ToString() == "true");
          return result;
        }
      }
      catch (Exception)
      {
        AppPercy.Log("BrowserStack executer failed");
      }
      return null;
    }

    internal JObject ExecutePercyScreenshotEnd(String name, String percyScreenshotUrl, String error)
    {
      try
      {
        if (markedPercySession)
        {
          String status = "success";
          var statusMessage = error;
          if (error != null)
          {
            status = "failure";

          }
          var obj = JObject.FromObject(new
          {
            action = "percyScreenshot",
            arguments = new
            {
              state = "end",
              percyScreenshotUrl = percyScreenshotUrl,
              status = status,
              statusMessage = statusMessage,
              name = name
            }
          });
          var resultString = percyAppiumDriver.ExecuteScript("browserstack_executor:" + obj.ToString());
          var result = JObject.Parse(resultString);
          markedPercySession = result["success"]?.ToString() == "true";
          return result;
        }
      }
      catch (Exception)
      {
        AppPercy.Log("BrowserStack executer failed", "debug");
      }
      return null;
    }

    public override String Screenshot(String name, String deviceName, int statusBarHeight, int navBarHeight,
        String orientation, Boolean fullScreen, String? platformVersion = null)
    {
      var result = ExecutePercyScreenshotBegin(name);
      var percyScreenshotUrl = "";
      String? error = null;
      var device = this.DeviceName(deviceName, result);
      SetDebugUrl(result);
      try
      {
        percyScreenshotUrl = base.Screenshot(name, device, statusBarHeight, navBarHeight, orientation, fullScreen, new List<string>(
        result.GetValue("osVersion")?.ToString().Split(new string[] { "\\." }, StringSplitOptions.None))[0]);
      }
      catch (Exception e)
      {
        error = e.Message;
      }
      ExecutePercyScreenshotEnd(name, percyScreenshotUrl, error);
      return "";
    }

    internal String? DeviceName(String deviceName, JObject result)
    {
      return deviceName ?? result?.GetValue("deviceName")?.ToString();
    }
  }
}
