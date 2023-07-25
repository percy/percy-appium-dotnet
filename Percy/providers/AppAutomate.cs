using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace PercyIO.Appium
{
  internal class AppAutomate : GenericProvider
  {
    private Boolean markedPercySession = true;
    private IPercyAppiumDriver percyAppiumDriver;

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

    internal String GetDebugUrl(JObject result)
    {
      if (result == null) return null;

      var buildHash = result.GetValue("buildHash").ToString();
      var sessionHash = result.GetValue("sessionHash").ToString();
      return "https://app-automate.browserstack.com/dashboard/v2/builds/" + buildHash + "/sessions/" + sessionHash;
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
              percyBuildId = Env.GetPercyBuildID(),
              percyBuildUrl = Env.GetPercyBuildUrl(),
              name = name
            }
          });
          var resultString = percyAppiumDriver.ExecuteScript("browserstack_executor:" + obj.ToString());
          var result = JObject.Parse(resultString);

          markedPercySession = (result.GetValue("success").IsTrue());
          return result;
        }
      }
      catch (Exception)
      {
        Utils.Log("BrowserStack executer failed");
      }
      return null;
    }

    internal JObject? ExecutePercyScreenshotEnd(String name, String percyScreenshotUrl, String? error)
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
          markedPercySession = result.GetValue("success")!.IsTrue();
          return result;
        }
      }
      catch (Exception)
      {
        Utils.Log("BrowserStack executer failed", "debug");
      }
      return null;
    }

    public override String Screenshot(String name, ScreenshotOptions options, String? platformVersion = null)
    {
      var result = ExecutePercyScreenshotBegin(name);
      var percyScreenshotUrl = "";
      String? error = null;
      options.DeviceName = this.DeviceName(options.DeviceName, result);
      base.SetDebugUrl(GetDebugUrl(result));
      try
      {
        percyScreenshotUrl = base.Screenshot(
          name,
          options,
          OsVersion(result)
        );
      }
      catch (Exception e)
      {
        error = e.Message;
        throw e;
      }
      finally
      {
        ExecutePercyScreenshotEnd(name, percyScreenshotUrl, error);
      }
      return "";
    }

    internal override List<Tile> CaptureTiles(ScreenshotOptions options)
    {
      // For single screens just use original approach
      if (Env.DisableRemoteUploads())
      {
        if (options.FullPage)
        {
          Utils.Log("Full page screenshots are only supported when \"isDisableRemoteUpload\" is not set", "warn");
        }
        return base.CaptureTiles(options);
      }

      var statusBar = this.metadata.StatBarHeight();
      var navBar = this.metadata.NavBarHeight();
      string reqObject = ExecutePercyScreenshot(options);
      var jsonarray = new JArray();
      try
      {
        jsonarray = JArray.Parse(reqObject);
      }
      catch (Exception e)
      {
        var error = e.Message;
        throw new Exception("Error", e);
      }
      List<Tile> tiles = new List<Tile>();
      foreach (JObject jsonobject in jsonarray)
      {
        String sha = jsonobject.GetValue("sha").ToString().Split('-')[0];
        int HeaderHeight = (int)jsonobject.GetValue("header_height");
        int FooterHeight = (int)jsonobject.GetValue("footer_height");
        tiles.Add(new Tile(null, statusBar, navBar, HeaderHeight, FooterHeight, options.FullScreen, sha));
      }
      return tiles;
    }

    internal string ExecutePercyScreenshot(ScreenshotOptions options)
    {
      var screenshotType = "fullpage";
      if (!options.FullPage || (options.ScreenLengths != null && options.ScreenLengths < 2) || !VerifyCorrectAppiumVersion())
      {
        screenshotType = "singlepage";
      }

      var projectId = "percy-prod";
      if (Env.EnablePercyDev())
      {
        projectId = "percy-dev";
      }
      var reqObject = JObject.FromObject(new
      {
        action = "percyScreenshot",
        arguments = new
        {
          state = "screenshot",
          percyBuildId = Env.GetPercyBuildID(),
          screenshotType = screenshotType,
          scaleFactor = this.metadata.ScaleFactor(),
          projectId = projectId,
          options = new
          {
            deviceHeight = this.metadata.DeviceScreenHeight(),
            numOfTiles = options.ScreenLengths,
            scollableXpath = options.ScrollableXpath,
            scrollableId = options.ScrollableId,
            FORCE_FULL_PAGE = Env.ForceFullPage()
          }
        }
      });

      var resultString = percyAppiumDriver.ExecuteScript(
        string.Format("browserstack_executor: {0}", reqObject.ToString())).ToString();
      JObject result = JObject.Parse(resultString);
      return result.GetValue("result").ToString();
    }

    internal String? DeviceName(String deviceName, JObject result)
    {
      return deviceName ?? result?.GetValue("deviceName")?.ToString();
    }

    internal String? OsVersion(JObject result)
    {
      if (result == null) return null;

      return new List<string>(result.GetValue("osVersion")?.ToString().Split(new string[] { "\\." }, StringSplitOptions.None))[0];
    }

    internal Boolean VerifyCorrectAppiumVersion()
    {
      var bstackOptions = percyAppiumDriver.GetCapabilities().getValue<Dictionary<string, object>>("bstack:options");
      var appiumVersionJsonProtocol = percyAppiumDriver.GetCapabilities().getValue<String>("browserstack.appium_version");
      if (bstackOptions == null && appiumVersionJsonProtocol == null)
      {
        Utils.Log("Unable to fetch Appium version, Appium version should be >= 1.19 for Fullpage Screenshot", "warn");
      }
      else if ((appiumVersionJsonProtocol != null && !AppiumVersionCheck(appiumVersionJsonProtocol)) || (bstackOptions != null && !AppiumVersionCheck(bstackOptions["appiumVersion"].ToString())))
      {
        Utils.Log("Appium version should be >= 1.19 for Fullpage Screenshot, Falling back to single page screenshot.", "warn");
        return false;
      }
      return true;
    }

    internal Boolean AppiumVersionCheck(String version)
    {
      string[] versionArr = version.Split('.');
      int majorVersion = int.Parse(versionArr[0]);
      int minorVersion = int.Parse(versionArr[1]);

      if (majorVersion == 2 || (majorVersion == 1 && minorVersion > 18))
      {
        return true;
      }
      return false;
    }
  }
}
