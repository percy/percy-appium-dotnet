using System;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using System.IO;

namespace PercyIO.Appium
{
  [ExcludeFromCodeCoverage]
  internal class CliWrapper
  {
    public static readonly string CLI_API = Environment.GetEnvironmentVariable("PERCY_CLI_API") ?? "http://localhost:5338";
    private static HttpClient _http = new HttpClient();
    private static bool? _enabled = null;

    private static dynamic Request(string endpoint, JObject? payload = null)
    {
      StringContent? body = payload == null ? null : new StringContent(
                payload.ToString(), Encoding.UTF8, "application/json");
      Task<HttpResponseMessage> apiTask = body != null
          ? _http.PostAsync($"{CLI_API}{endpoint}", body)
          : _http.GetAsync($"{CLI_API}{endpoint}");
      apiTask.Wait();

      HttpResponseMessage response = apiTask.Result;
      response.EnsureSuccessStatusCode();

      Task<string> contentTask = response.Content.ReadAsStringAsync();
      contentTask.Wait();

      IEnumerable<string>? version = null;
      response.Headers.TryGetValues("x-percy-core-version", out version);

      return new
      {
        version = version == null ? null : version.First(),
        content = contentTask.Result
      };
    }

    internal static Func<bool> Healthcheck = () =>
    {
      if (_enabled != null) return (bool)_enabled;

      try
      {
        dynamic res = Request("/percy/healthcheck");
        dynamic data = DeserializeJson<dynamic>(res.content);
        Env.SetPercyBuildID(data.build.id.ToString());
        Env.SetPercyBuildUrl(data.build.url.ToString());
        Env.SetSessionType(data.type.ToString());
        string[] version = res.version.Split('.');
        int majorVersion = int.Parse(version[0]);
        int minorVersion = int.Parse(version[1]);

        if (data.success.ToString().ToLower() != "true")
        {
          throw new Exception(data.error);
        }

        if (majorVersion < 1)
        {
          Utils.Log($"Unsupported Percy CLI version, {res.version}");
          return (bool)(_enabled = false);
        }
        else
        {
          if (minorVersion < 26) {
            Utils.Log($"Percy CLI version, {res.version} " +
            "is not minimum version required, Percy on Automate is available from 1.27.0-beta.0.", "warn");
            return (bool)(_enabled = false);
          }
        }
        return (bool)(_enabled = true);
      }
      catch (Exception error)
      {
        Utils.Log("Percy is not running, disabling snapshots");
        Utils.Log(error.ToString());
        return (bool)(_enabled = false);
      }
    };

    internal static String PostScreenshot(string name, JObject tag, List<Tile> tiles, String externalDebugUrl, JObject ignoredElementsData, JObject consideredElementsData)
    {
      try
      {
        var screenshotOptions = new
        {
          clientInfo = Env.GetClientInfo(),
          environmentInfo = Env.GetEnvironmentInfo(),
          tag = tag,
          tiles = Tile.GetTilesAsJson(tiles),
          externalDebugUrl = externalDebugUrl,
          name = name,
          ignoredElementsData = ignoredElementsData,
          consideredElementsData = consideredElementsData
        };
        dynamic res = Request("/percy/comparison", JObject.FromObject(screenshotOptions));
        dynamic data = DeserializeJson<dynamic>(res.content);
        if (data.success.ToString().ToLower() != "true")
        {
          throw new Exception(data.error.ToString());
        }
        return data.link.ToString();
      }
      catch (Exception error)
      {
        Utils.Log($"Could not take screenshot \"{name}\"");
        Utils.Log(error.ToString(), "debug");
        return null;
      }
    }

    internal static void PostPOAScreenshot(string name, string sessionId, string commandExecutorUrl, IPercyAppiumCapabilities capabilities, Dictionary<string, object> options)
    {
      try
      {
        var screenshotOptions = new
        {
          clientInfo = Env.GetClientInfo(),
          environmentInfo = Env.GetEnvironmentInfo(),
          sessionId = sessionId,
          commandExecutorUrl = commandExecutorUrl,
          capabilities = capabilities.GetCapabilities(),
          snapshotName = name,
          options = JObject.FromObject(options)
        };
        dynamic res = Request("/percy/automateScreenshot", JObject.FromObject(screenshotOptions));
        dynamic data = DeserializeJson<dynamic>(res.content);
        if (data.success.ToString().ToLower() != "true")
        {
          throw new Exception(data.error.ToString());
        }
      }
      catch (Exception error)
      {
        Utils.Log($"Could not take screenshot \"{name}\"");
        Utils.Log(error.ToString());
      }
    }

    private static T DeserializeJson<T>(string json)
    {
        JsonSerializer serializer = new JsonSerializer();
        using (JsonTextReader reader = new JsonTextReader(new StringReader(json)))
        {
            return serializer.Deserialize<T>(reader);
        }
    }

    internal static void setHttpClient(HttpClient client)
    {
      _http = client;
    }

    internal static void resetHttpClient()
    {
      _http = new HttpClient();
    }
  }
}
