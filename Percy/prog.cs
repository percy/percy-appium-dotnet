using System;
using System.Collections.Generic;
using System.Threading;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Appium.iOS;
using OpenQA.Selenium.Support.UI;
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
      //browserstackOptions.Add("appiumVersion", "1.19.1");
      browserstackOptions.Add("appiumVersion", "1.22.0");




      // Percy options
      Dictionary<string, string> percyOtions = new Dictionary<string, string>();
      percyOtions.Add("ignoreErrors", "false");
      percyOtions.Add("enabled", "true");
      // Adding capabilities
      capabilities.AddAdditionalCapability("bstack:options", browserstackOptions);
      capabilities.AddAdditionalCapability("appium:percyOptions", percyOtions);
      // Adding Device
      //for ios
      //capabilities.AddAdditionalCapability("appium:autoAcceptAlerts", "true");
      // for android
      capabilities.AddAdditionalCapability("appium:autoGrantPermissions", "true");

      // Adding app that was uploaded
      // RN Android
      capabilities.AddAdditionalCapability("appium:app", "bs://361abd112d63e1d96c4a34f538a7b390594c3956");
      // CNN Android
      //capabilities.AddAdditionalCapability("appium:app", "bs://920b3e63bfa043a1c58c1bf2ab4c5ab95439dfd7");

      // ios
      //capabilities.AddAdditionalCapability("appium:app", "bs://d2dda94e809e3a2bf103e11f63b19c907894af30");
      // Project details
      capabilities.AddAdditionalCapability("project", "CNN Project debug");
      capabilities.AddAdditionalCapability("build", "CSharp Android CNN");
      capabilities.AddAdditionalCapability("name", "first_test");

      // string[] deviceName = { "iphone 11", "iphone 13", "iphone 11" };
      // string[] version = { "15", "15", "14" };
      string[] deviceName = { "Google Pixel 6" };
      string[] version = { "12.0" };
      // Initialize the remote Webdriver using BrowserStack remote URL
      // and desired capabilities defined above
      for (int i = 0; i < 1; i++)
      {
        capabilities.AddAdditionalCapability("platformVersion", version[i]);
        capabilities.AddAdditionalCapability("appium:deviceName", deviceName[i]);

        // IOSDriver<IOSElement> driver = new IOSDriver<IOSElement>(
        //         new Uri("https://hub-cloud.browserstack.com/wd/hub"), capabilities);

        AndroidDriver<AndroidElement> driver = new AndroidDriver<AndroidElement>(
                new Uri("https://hub-cloud.browserstack.com/wd/hub"), capabilities);

        // Initialize AppPercy
        AppPercy appPercy = new AppPercy(driver);

        ScreenshotOptions options = new ScreenshotOptions();
        // options.FullPage = true;
        // options.ScreenLengths = 2;


        // // CNN
        // var xpath1 = "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.view.ViewGroup/android.view.View/android.widget.ScrollView/android.view.View[5]";
        // var xpath2 = "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.view.ViewGroup/android.view.View/android.view.View/android.view.View/android.view.View[1]";
        // var xpath3 = "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/androidx.appcompat.widget.LinearLayoutCompat/android.widget.ScrollView/android.widget.LinearLayout/android.widget.Button";
        // try
        // {
        //   AndroidElement element = driver.FindElementByXPath(xpath1);
        //   element.Click();
        // }
        // catch (Exception e)
        // {
        //   Console.WriteLine("Element 1 Not found");
        // }
        // appPercy.Screenshot("CNN Alert", options);

        // try
        // {
        //   AndroidElement element = driver.FindElementByXPath(xpath2);
        //   element.Click();
        // }
        // catch (Exception e)
        // {
        //   Console.WriteLine("Element 1 Not found");
        // }
        // try
        // {
        //   AndroidElement element = driver.FindElementByXPath(xpath3);
        //   element.Click();
        // }
        // catch (Exception e)
        // {
        //   Console.WriteLine("Element 1 Not found");
        // }
        // appPercy.Screenshot("CNN Homepage", options);

        // RN
        //var xpath = "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.view.ViewGroup/android.view.ViewGroup/android.view.ViewGroup/android.view.ViewGroup/android.view.ViewGroup";
        try
        {
          // AndroidElement element = driver.FindElementsByAccessibilityId("Login");
          var usernamexpath = "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.view.ViewGroup/android.view.ViewGroup/android.view.ViewGroup/android.view.ViewGroup/android.widget.EditText[1]";
          var passxpath = "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.view.ViewGroup/android.view.ViewGroup/android.view.ViewGroup/android.view.ViewGroup/android.widget.EditText[2]";
          AndroidElement username = driver.FindElementByXPath(usernamexpath);
          AndroidElement password = driver.FindElementByXPath(passxpath);
          username.SendKeys("Praduasdkasdhaskjfskjdaam");
          password.SendKeys("Pradumasdasdasdasdasdasdjasdkhsdkhj");
          AndroidElement element = driver.FindElementByAccessibilityId("Login");
          List<String> ids = new List<string>();
          List<String> xpaths = new List<string>();
          List<AppiumWebElement> appiumWebElements = new List<AppiumWebElement>();
          appiumWebElements.Add(password);
          ids.Add("Login");
          xpaths.Add(usernamexpath);
          options.Xpaths = xpaths;
          options.AppiumElements = appiumWebElements;

          options.AccessibilityIds = ids;
          element.Click();
          //AndroidElement element = driver.FindElementByXPath(xpath);


        }
        catch (Exception e)
        {
          Console.WriteLine("Element 1 Not found");
        }


        appPercy.Screenshot("Take Snaps", options);
        //var wait = new WebDriverWait(driver, new TimeSpan(0, 0, 30));

        // var xpath = "/XCUIElementTypeApplication/XCUIElementTypeWindow/XCUIElementTypeOther/XCUIElementTypeOther/XCUIElementTypeOther/XCUIElementTypeOther/XCUIElementTypeOther[2]/XCUIElementTypeOther[2]/XCUIElementTypeOther[2]/XCUIElementTypeOther[3]/XCUIElementTypeOther[2]/XCUIElementTypeOther[2]/XCUIElementTypeButton";
        // var element = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(MobileBy.XPath(xpath)));
        // element.Click();
        // var xpath1 = "/XCUIElementTypeApplication/XCUIElementTypeWindow/XCUIElementTypeOther/XCUIElementTypeOther/XCUIElementTypeOther/XCUIElementTypeOther/XCUIElementTypeOther[2]/XCUIElementTypeOther[2]/XCUIElementTypeOther[2]/XCUIElementTypeOther[3]/XCUIElementTypeOther[2]/XCUIElementTypeOther[1]/XCUIElementTypeStaticText";
        // var element1 = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(MobileBy.XPath(xpath1)));
        // element1.Click();
        // try
        // {
        //   var element = driver.FindElementByAccessibilityId("login_btn");
        //   Console.WriteLine(element.Location);
        //   // appPercy.Screenshot("Take Snaps", options);
        // }
        // catch (Exception e)
        // {
        //   Console.WriteLine("erro");
        // }
        // Console.WriteLine(driver.SessionId.ToString());
        // REDDIT
        // var xpath = "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.RelativeLayout/android.widget.FrameLayout/android.view.ViewGroup/android.view.View/android.view.View/android.widget.Button";
        // try{
        //   AndroidElement element = driver.FindElementByXPath(xpath);
        //   element.Click();
        // } catch (Exception e) {
        //   Console.WriteLine("Element 1 Not found");
        // }
        // var xpath1 = "/hierarchy/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.ScrollView/android.widget.LinearLayout/android.widget.LinearLayout/android.widget.LinearLayout[2]/android.widget.Button[1]";
        // try{
        //   AndroidElement elements = driver.FindElementByXPath(xpath1);
        //   elements.Click();
        // } catch (Exception e) {
        //   Console.WriteLine("Element 2 Not found");
        // }
        // MCD
        // var xpath = "/hierarchy/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.ScrollView/android.widget.LinearLayout/android.widget.LinearLayout/android.widget.LinearLayout[2]/android.widget.Button[1]";
        // try{
        //   AndroidElement element = driver.FindElementByXPath(xpath);
        //   element.Click();
        // } catch (Exception e) {
        //   Console.WriteLine("Element 1 Not found");
        // }
        // var xpath1 = "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.view.ViewGroup/android.widget.LinearLayout/android.widget.LinearLayout[1]/android.widget.RelativeLayout";
        // try{
        // AndroidElement elements = driver.FindElementByXPath(xpath1);
        // Console.WriteLine(elements.Location);
        // Console.WriteLine(elements.Size);
        // elements.Click();
        // } catch (Exception e) {
        //   Console.WriteLine("Element 2 Not found");
        // }
        // var xpath2 = "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.view.ViewGroup/androidx.recyclerview.widget.RecyclerView/android.widget.LinearLayout[10]";
        // try{
        // AndroidElement element1 = driver.FindElementByXPath(xpath2);
        // Console.WriteLine(element1.Location);
        // Console.WriteLine(element1.Size);
        // //element1.Click();
        // } catch (Exception e) {
        //   Console.WriteLine("Element 3 Not found");
        // }
        // var xpath3 = "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.view.ViewGroup/androidx.recyclerview.widget.RecyclerView/android.widget.LinearLayout[11]";
        // try{
        // AndroidElement element2 = driver.FindElementByXPath(xpath3);
        // Console.WriteLine(element2.Location);
        // Console.WriteLine(element2.Size);
        // //element1.Click();
        // } catch (Exception e) {
        //   Console.WriteLine("Element  Not found");
        // }
        // appPercy.Screenshot("Take Snaps");
        // Get all available contexts
        // List<string> availableContexts = driver.Contexts.ToList();

        // // Find the Flutter context
        // string flutterContext = availableContexts.FirstOrDefault(context => context.StartsWith("FLUTTER"));

        // // Switch to the Flutter context
        // driver.Context = flutterContext;
        //   List<string> AllContexts = new List<string>();
        //   foreach (var context in (driver.Contexts))
        //   {
        //       AllContexts.Add(context);
        //   }
        //  // driver.Context = (AllContexts[1]);
        //   try{
        //     AndroidElement element = driver.FindElementById("login_btn");
        //    // AndroidElement element = driver.FindElementByAccessibilityId("login_btn");
        //     Console.WriteLine(element.Location);
        //     Console.WriteLine(element.Size);
        //   } catch (Exception e) {
        //     Console.WriteLine(e);
        //   }


        // Invoke driver.quit() after the test is done to indicate that the test is completed.
        driver.Quit();
      }
    }
  }
}