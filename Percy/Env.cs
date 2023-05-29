using System;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Appium.iOS;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;

namespace PercyIO.Appium
{
  class Env
  {
    private static string percyBuildID = null;
    private static string percyBuildUrl = null;

    internal static String GetPercyBuildID()
    {
      return percyBuildID;
    }

    internal static String GetPercyBuildUrl()
    {
      return percyBuildUrl;
    }

    internal static void SetPercyBuildID(String percyBuildIDParam)
    {
      percyBuildID = percyBuildIDParam;
    }

    internal static void SetPercyBuildUrl(String percyBuildUrlParam)
    {
      percyBuildUrl = percyBuildUrlParam;
    }

    internal static String GetClientInfo()
    {
      return "percy-appium-dotnet/2.0.1";
    }

    internal static String GetEnvironmentInfo()
    {
      return Regex.Replace(
        Regex.Replace(RuntimeInformation.FrameworkDescription, @"\s+", "-"), @"-([\d\.]+).*$", "/$1"
      ).Trim().ToLower();
    }
  }
}
