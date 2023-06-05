using System;

namespace PercyIO.Appium
{
  internal class Utils
  {

    public static readonly string[] SupportedDriverClassnames = { "OpenQA.Selenium.Appium.Android.AndroidDriver", "OpenQA.Selenium.Appium.iOS.IOSDriver" };
    
    public static Boolean isValidDriverObject(Object obj)
    {
      String type = obj.GetType().ToString();
      foreach (string constant in SupportedDriverClassnames)
      {
        if (type.Contains(constant))
        {
          return true;
        }
      }

      return false;
    }
  }
}