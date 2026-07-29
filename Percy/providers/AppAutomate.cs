using System;
using System.Collections.Generic;
using System.Globalization;
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
      if (!string.IsNullOrEmpty(remoteAddress) &&
          remoteAddress.Contains(Environment.GetEnvironmentVariable("AA_DOMAIN") ?? "browserstack"))
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
      catch (Exception e)
      {
        Utils.Log("BrowserStack executor failed at percyScreenshot begin");
        Utils.Log(e.ToString(), "debug");
      }
      return null;
    }

    internal JObject? ExecutePercyScreenshotEnd(String name, String percyScreenshotUrl, Boolean? sync, String? error)
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
              name = name,
              sync = sync
            }
          });
          var resultString = percyAppiumDriver.ExecuteScript("browserstack_executor:" + obj.ToString());
          var result = JObject.Parse(resultString);
          markedPercySession = result.GetValue("success")!.IsTrue();
          return result;
        }
      }
      catch (Exception e)
      {
        // End is what reports failure status back to the hub, and that statusMessage is often
        // the last surviving record of a failed screenshot — so do not lose why End itself failed.
        Utils.Log("BrowserStack executor failed at percyScreenshot end");
        Utils.Log(e.ToString(), "debug");
      }
      return null;
    }

    public override JObject Screenshot(String name, ScreenshotOptions options, String? platformVersion = null)
    {
      var result = ExecutePercyScreenshotBegin(name);
      var percyScreenshotUrl = "";
      String? error = null;
      options.DeviceName = this.DeviceName(options.DeviceName, result);
      base.SetDebugUrl(GetDebugUrl(result));
      try
      {
        JObject data = base.Screenshot(
          name,
          options,
          OsVersion(result)
        );

        percyScreenshotUrl = data?.GetValue("link")?.ToString();
        return data;
      }
      catch (Exception e)
      {
        // `throw;` not `throw e;` — the latter resets the stack trace to this line and hides
        // where the failure actually came from.
        // statusMessage is persisted into the hub session log, so redact there too.
        error = Utils.RedactCredentials(e.Message);
        throw;
      }
      finally
      {
        ExecutePercyScreenshotEnd(name, percyScreenshotUrl, options.Sync, error);
      }
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
        // "Error" told a reader nothing about which stage failed; say what could not be parsed.
        throw new Exception(
          $"Could not parse tile data returned by the percyScreenshot executor: {e.Message}", e);
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
            topScrollviewOffset = options.TopScrollviewOffset,
            bottomScrollviewOffset = options.BottomScrollviewOffset,
            iosOptimizedFullpage = options.IosOptimizedFullpage,
            FORCE_FULL_PAGE = Env.ForceFullPage()
          }
        }
      });

      var resultString = percyAppiumDriver.ExecuteScript(
        string.Format("browserstack_executor: {0}", reqObject.ToString())).ToString();
      JObject result = JObject.Parse(resultString);

      // The hub reports a refusal as {"success": false, "message": ...} with no "result" key.
      // Indexing it blindly turned that into a bare NullReferenceException and threw away the
      // hub's explanation — the same undiagnosable failure this change exists to remove. It
      // matters more now than it did: fullpage is attempted whenever the version cannot be
      // determined, and on a real session the version is never present in the response
      // capabilities, so this is the path essentially every fullpage request takes.
      JToken? payload = result.GetValue("result");
      if (payload == null)
      {
        // Distinguish an actual refusal from a malformed success. Reporting "refused by
        // BrowserStack" for a `success: true` response missing `result` would send users looking
        // for a permission problem they do not have — the same misdirection as the old
        // "should be >= 1.19" message this branch exists to avoid.
        String? message = result.GetValue("message")?.ToString();
        bool refused = result.GetValue("success")?.Type == JTokenType.Boolean
          && result.GetValue("success")!.Value<bool>() == false;
        throw new Exception(refused
          ? $"percyScreenshot {screenshotType} was refused by BrowserStack: {message ?? resultString}"
          : $"percyScreenshot {screenshotType} returned no result: {message ?? resultString}");
      }
      return payload.ToString();
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

    // One policy throughout: downgrade to single page only when the version is known to be
    // below the gate. Every "we could not determine it" case attempts fullpage, which is what
    // the no-capabilities branch has always done and what percy-appium-java does.
    internal Boolean VerifyCorrectAppiumVersion()
    {
      try
      {
        var bstackOptions = percyAppiumDriver.GetCapabilities().getValue<Dictionary<string, object>>("bstack:options");
        // Fetched as object, not String: getValue<T> returns default(T) when the stored value is
        // not a T, so an unquoted `browserstack.appium_version: 1.16` came back null and read as
        // "not present" — silently skipping the gate for a version below it. The loop below
        // applies the same string/integral/floating-point handling to both protocols.
        object? appiumVersionJsonProtocol = percyAppiumDriver.GetCapabilities().getValue<object>("browserstack.appium_version");

        // `bstack:options` is present on every W3C session — the BrowserStack SDK always injects
        // it — but `appiumVersion` is only inside it when the user pins one, so the key has to be
        // probed rather than indexed. Indexing a missing key threw KeyNotFoundException out of
        // this fullpage-only branch and surfaced to users as Screenshot() returning null with no
        // snapshot ever posted.
        object? bstackAppiumVersion =
          (bstackOptions != null && bstackOptions.ContainsKey("appiumVersion"))
            ? bstackOptions["appiumVersion"]
            : null;

        // Both protocols are consulted: either one known to be below the gate downgrades. Taking
        // only the first non-null would let an unparseable JWP value hide a usable W3C one.
        // Track whether anything was seen but could not be judged, so the reassurance is emitted
        // once after the loop rather than promised per-iteration and then contradicted by a
        // later downgrade. The loop never exits early: breaking on the first usable value would
        // let an above-gate JWP value hide a below-gate W3C one, the same shadowing bug in the
        // other direction.
        bool undetermined = false;

        foreach (var raw in new object?[] { appiumVersionJsonProtocol, bstackAppiumVersion })
        {
          if (raw == null) continue;

          // Integral types convert losslessly, so accept them: `appiumVersion: 2` unquoted in the
          // yml is a long, and rejecting it would both warn on every fullpage snapshot and stop
          // the gate being enforced for `appiumVersion: 1`. Floating point is the lossy case —
          // `appiumVersion: 1.20` arrives as the double 1.2, and rebuilding "1.2" would compare
          // minor 2 against the gate and wrongly downgrade a version well above it.
          String? declaredVersion = raw as string;
          if (declaredVersion == null && IsIntegral(raw))
          {
            declaredVersion = Convert.ToString(raw, CultureInfo.InvariantCulture);
          }
          if (declaredVersion == null)
          {
            Utils.Log($"Ignoring non-string Appium version capability '{Convert.ToString(raw, CultureInfo.InvariantCulture)}'" +
              " — quote appiumVersion in browserstack.yml.", "warn");
            undetermined = true;
            continue;
          }

          Boolean? meetsGate = AppiumVersionCheck(declaredVersion);
          if (meetsGate == null)
          {
            // Say what could not be parsed. Reporting this as "should be >= 1.19" sent users
            // looking for a version problem they did not have.
            Utils.Log($"Could not parse Appium version '{declaredVersion}'.", "warn");
            undetermined = true;
            continue;
          }
          if (meetsGate == false)
          {
            Utils.Log("Appium version should be >= 1.19 for Fullpage Screenshot, Falling back to single page screenshot.", "warn");
            return false;
          }
          // Above the gate. Keep consulting the other protocol so a below-gate value there still
          // downgrades.
        }

        // Not guarded on whether some other value parsed: the downgrade above is the only one and
        // here means fullpage will be attempted and the reassurance cannot be contradicted.
        // Suppressing it when another protocol happened to parse would leave a lone "Ignoring
        // non-string..." or "Could not parse..." line with no stated consequence.
        if (undetermined)
        {
          Utils.Log("Attempting Fullpage Screenshot anyway.", "warn");
        }

        // Only when the session exposes no capabilities at all. Not pinning `appiumVersion` is
        // the common case, and warning on it would fire on every fullpage snapshot of every
        // build to say that nothing is wrong.
        if (bstackOptions == null && appiumVersionJsonProtocol == null)
        {
          Utils.Log("Unable to fetch Appium version, attempting Fullpage Screenshot anyway.", "warn");
        }
        return true;
      }
      catch (Exception e)
      {
        // Version detection must never take the screenshot down with it. Attempt fullpage rather
        // than downgrade: reflection handing back a null capability map (Appium 8.x) would
        // otherwise turn into a permanent silent single-page fallback, which diffs against
        // fullpage baselines and looks like a passing build.
        Utils.Log("Unable to verify Appium version, attempting Fullpage Screenshot anyway.", "warn");
        Utils.Log(e.ToString(), "debug");
        return true;
      }
    }

    private static Boolean IsIntegral(object value)
    {
      return value is sbyte || value is byte || value is short || value is ushort
          || value is int || value is uint || value is long || value is ulong;
    }

    // null when the version cannot be determined, otherwise whether it meets the >= 1.19 gate.
    internal Boolean? AppiumVersionCheck(String version)
    {
      if (String.IsNullOrWhiteSpace(version))
      {
        return null;
      }

      // A pinned version can legitimately be major-only ("2"), so treat a missing minor as 0
      // instead of indexing past the end of the array.
      string[] versionArr = version.Split('.');
      if (!int.TryParse(versionArr[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out int majorVersion))
      {
        return null;
      }
      int minorVersion = 0;
      if (versionArr.Length > 1
          && !int.TryParse(versionArr[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out minorVersion))
      {
        // A present-but-unparseable minor ("1.19-beta", "1.x") means the version cannot be
        // determined — the same as an unparseable major — not that it is below the gate.
        return null;
      }

      // The gate is "Appium >= 1.19". This was written as `== 2` when 2.x was the newest major,
      // so Appium 3.x — which BrowserStack now offers and defaults to on newer devices — failed a
      // check it comfortably satisfies, silently downgrading every fullpage request to single
      // page. Compare as >= 2 so future majors don't regress the same way.
      if (majorVersion >= 2 || (majorVersion == 1 && minorVersion > 18))
      {
        return true;
      }
      return false;
    }
  }
}
