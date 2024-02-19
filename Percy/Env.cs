using System;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;

namespace PercyIO.Appium
{
  class Env
  {
    private static string percyBuildID = null;
    private static string percyBuildUrl = null;
    private static string sessionType = null;

    internal static String GetPercyBuildID()
    {
      return percyBuildID;
    }

    internal static String GetPercyBuildUrl()
    {
      return percyBuildUrl;
    }

    internal static String GetSessionType()
    {
      return sessionType;
    }

    internal static void SetPercyBuildID(String percyBuildIDParam)
    {
      percyBuildID = percyBuildIDParam;
    }

    internal static void SetPercyBuildUrl(String percyBuildUrlParam)
    {
      percyBuildUrl = percyBuildUrlParam;
    }

    internal static void SetSessionType(String type)
    {
      sessionType = type;
    }

    internal static String GetClientInfo()
    {
      return "percy-appium-dotnet/3.0.4";
    }

    internal static Boolean ForceFullPage()
    {
      return Environment.GetEnvironmentVariable("FORCE_FULL_PAGE") == "true";
    }

    internal static Boolean DisableRemoteUploads()
    {
      return Environment.GetEnvironmentVariable("PERCY_DISABLE_REMOTE_UPLOADS") == "true";
    }

    internal static Boolean EnablePercyDev()
    {
      return Environment.GetEnvironmentVariable("PERCY_ENABLE_DEV") == "true";
    }

    internal static String GetEnvironmentInfo()
    {
      return Regex.Replace(
        Regex.Replace(RuntimeInformation.FrameworkDescription, @"\s+", "-"), @"-([\d\.]+).*$", "/$1"
      ).Trim().ToLower();
    }
  }
}
