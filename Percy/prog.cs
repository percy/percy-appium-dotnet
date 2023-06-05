using System;
using System.Collections.Generic;
using System.Threading;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.iOS;
using PercyIO.Appium;

namespace csharp_appium__w3c_first_ios_test_browserstack
{
  public class Ios
  {
    static void Main(string[] args)
    {
      //w3c
      AppiumOptions capabilities = new AppiumOptions();
      // Bstack options
      Dictionary<string, object> browserstackOptions = new Dictionary<string, object>();
      browserstackOptions.Add("userName", "pradumkumar_USRpXW");
      browserstackOptions.Add("accessKey", "3oYGPpwSxJpxpWKpzYEg");
      browserstackOptions.Add("appiumVersion", "2.0.0");

      //browserstackOptions.Add("appiumVersion", "2.0.0");

      // Percy options
      Dictionary<string, string> percyOtions = new Dictionary<string, string>();
      percyOtions.Add("ignoreErrors", "true");
      percyOtions.Add("enabled", "true");
      // Adding capabilities
      capabilities.AddAdditionalAppiumOption("bstack:options", browserstackOptions);
      capabilities.AddAdditionalAppiumOption("appium:percyOptions", percyOtions);
      // Adding Device
      capabilities.App = "bs://d1071384249085e3be61c7a774b557fe6c6b6a37";
      capabilities.AddAdditionalAppiumOption("bstack:options", browserstackOptions);
      capabilities.AddAdditionalAppiumOption("appium:percyOptions", percyOtions);
      // Adding Device
      capabilities.DeviceName = "iPhone 14";
      capabilities.PlatformVersion = "16";
      capabilities.AddAdditionalAppiumOption("project", "First CSharp W3C Project");
      capabilities.AddAdditionalAppiumOption("build", "CSharp IOS");
      capabilities.AddAdditionalAppiumOption("name", "first_test");

      // Initialize the remote Webdriver using BrowserStack remote URL
      // and desired capabilities defined above
      AppiumDriver driver = new IOSDriver(
              new Uri("https://hub-cloud.browserstack.com/wd/hub"), capabilities);

      // Initialize AppPercy
      List<String> xpath = new List<string>();
      xpath.Add("/XCUIElementTypeApplication/XCUIElementTypeWindow/XCUIElementTypeOther/XCUIElementTypeOther/XCUIElementTypeOther/XCUIElementTypeOther/XCUIElementTypeOther/XCUIElementTypeOther/XCUIElementTypeOther/XCUIElementTypeOther/XCUIElementTypeOther/XCUIElementTypeButton[1]");
      ScreenshotOptions options = new ScreenshotOptions();
      AppPercy appPercy = new AppPercy(driver);
      var ele = driver.FindElement("xpath", xpath[0]);
      Thread.Sleep(1000);
      ele.Click();
      Thread.Sleep(1000);
      appPercy.Screenshot("First Screenshot", options);
      // Invoke driver.quit() after the test is done to indicate that the test is completed.
      driver.Quit();
    }
  }
}
