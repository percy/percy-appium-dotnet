# percy-appium-dotnet
![Test](https://github.com/percy/percy-appium-dotnet/workflows/Test/badge.svg)

[Percy](https://percy.io) visual testing for .NET Selenium.

## NuGet

NuGet Package: [](http://www.nuget.org/packages/PercyIO.Appium/)

Dependencies:

- [Newtonsoft.Json](http://www.nuget.org/packages/Newtonsoft.Json/) Version >= 13.0.1
- [Castle.Core](https://www.nuget.org/packages/Castle.Core/) Version >= 4.3.1
- [Microsoft.CSharp](https://www.nuget.org/packages/Microsoft.CSharp) Version >= 4.7.0

> Note: This package is tested against .netstandard 2.0 should be compatible with and standard that is superset of this.

## Installation

npm install `@percy/cli` (requires Node 14+):

```sh-session
$ npm install --save-dev @percy/cli
```

Install the PercyIO.Appium package (for example, with .NET CLI):

```sh-session
$ dotnet add package PercyIO.Appium
```

## Usage

This is an example test using the `Percy.Snapshot` method.

``` csharp
// ... other test code
// import
using PercyIO.Appium;
class Program
{
  private static AppPercy percy;
  static void Main(string[] args)
  {
    AppiumOptions capabilities = new AppiumOptions();
    // Add caps here
    AndroidDriver<AndroidElement> driver = new AndroidDriver<AndroidElement>(
          new Uri("http://hub-cloud.browserstack.com/wd/hub"), capabilities);
​
    // take a snapshot
    appPercy = new AppPercy(driver);
    appPercy.Screenshot("dotnet snaphot-1", null);

    // quit driver
    driver.quit();
  }
}
```

Running the above normally will result in the following log:

```sh-session
[percy] Percy is not running, disabling snapshots
```

When running with [`percy
exec`](https://github.com/percy/cli/tree/master/packages/cli-exec#percy-exec), and your project's
`PERCY_TOKEN`, a new Percy build will be created and snapshots will be uploaded to your project.

```sh-session
$ export PERCY_TOKEN=[your-project-token]
$ percy app:exec -- [your dotnet test command]
[percy] Percy has started!
[percy] Created build #1: https://percy.io/[your-project]
[percy] Screenshot taken "dotnet snaphot-1"
[percy] Stopping percy...
[percy] Finalized build #1: https://percy.io/[your-project]
[percy] Done!
```

## Configuration

The screenshot method arguments:

``` csharp
  ScreenshotOptions options = new ScreenshotOptions();
  // Set options here
  percy.screenshot(name, fullScreen, options)
```

- `name` (**required**) - The screenshot name; must be unique to each screenshot
- Additional screenshot options (overrides any project options):
  - `fullScreen ` - (**optional**) It indicates if the app is a full screen
  - `options` - (**optional**) configure screenshot using below options:

| Screenshot Options | Type  | Description |
| ------------- | ------------- | ------------- |
| DeviceName | String  | Device name on which screenshot is taken  |
| StatusBarHeight | Int  | Height of status bar for the device  |
| NavBarHeight | Int  | Height of navigation bar for the device  |
| Orientation | ["portrait"/"landscape"]  | Orientation of the application  |
| FullPage | Boolean  | [Alpha] Only supported on App Automate driver sessions [ needs @percy/cli 1.20.2+ ]  |
| ScreenLengths | Int  | [Alpha] Max screen lengths for fullPage [ needs @percy/cli 1.20.2+ ]  |
| TopScrollviewOffset | Int  | [Alpha] Offset from top of scrollview [ needs @percy/cli 1.20.2+ ]  |
| BottomScrollviewOffset | Int  | [Alpha] Offset from bottom of scrollview [ needs @percy/cli 1.20.2+ ]  |
| FullScreen | Boolean  | Indicate whether app is full screen; boolean  |
| ScrollableXpath | String  | [Alpha] Scrollable element xpath for fullpage [ needs @percy/cli 1.20.2+ ]  |
| ScrollableId | String  | [Alpha] Scrollable element accessibility id for fullpage [ needs @percy/cli 1.20.2+ ]  |
| IgnoreRegionXpaths | list of string  | Elements xpaths that user want to ignore in visual diff [ needs @percy/cli 1.23.0+ ]  |
| IgnoreRegionAccessibilityIds | list of string  | Elements accessibility_ids that user want to ignore in visual diff [ needs @percy/cli 1.23.0+ ]  |
| IgnoreRegionAppiumElements | list of appium element object  | Appium elements that user want to ignore in visual diff [ needs @percy/cli 1.23.0+ ]  |
| CustomIgnoreRegions |  list of ignore_region object  | Custom locations that user want to ignore in visual diff [ needs @percy/cli 1.23.0+ ] <br /> - Description: IgnoreRegion class represents a rectangular area on a screen that needs to be ignored for visual diff. <br /> ```var ignoreRegion = new IgnoreRegion();```<br />```ignoreRegion.setTop() = top;``` <br />```ignoreRegion.setBottom = bottom;``` <br />```ignoreRegion.setLeft = left;``` <br />```ignoreRegion.setRight = right;```  |

## Running with Hybrid Apps

For a hybrid app, we need to switch to native context before taking screenshot.

- Add a helper method similar to following for say flutter based hybrid app:
```csharp
public void PercyScreenshotFlutter(AppPercy appPercy, AndroidDriver<AndroidElement> driver, String name, ScreenshotOptions options) {
    // switch to native context
    driver.Context = "NATIVE_APP";
    appPercy.Screenshot(name, options);
    // switch back to flutter context
    driver.Context = "FLUTTER";
}
```

- Call PercyScreenshotFlutter helper function when you want to take screenshot.
```csharp
PercyScreenshotFlutter(appPercy, driver, name, options);
```

> Note: 
>
> For other hybrid apps the `driver.Context = "FLUTTER";` would change to context that it uses like say WEBVIEW etc.
>

### Migrating Config

If you have a previous Percy configuration file, migrate it to the newest version with the
[`config:migrate`](https://github.com/percy/cli/tree/master/packages/cli-config#percy-configmigrate-filepath-output) command:

```sh-session
$ percy config:migrate
```

## Running Percy on Automate
`percyScreenshot(driver, name, options)` [ needs @percy/cli 1.27.0-beta.0+ ];

This is an example test using the `Percy.Screenshot` method.

``` csharp
// ... other test code
// import
using PercyIO.Appium;
class Program
{
  static void Main(string[] args)
  {

    // Add caps here
    RemoteWebDriver driver = new RemoteWebDriver(
      new Uri("https://hub-cloud.browserstack.com/wd/hub"),capabilities);
​
    // take a snapshot
    PercyOnAutomate Percy = new PercyOnAutomate(driver);

    // navigate to webpage
    driver.Navigate().GoToUrl("https://www.percy.io");

    Percy.Screenshot("dotnet screenshot-1");

    // quit driver
    driver.quit();
  }
}
```

- `driver` (**required**) - A appium driver instance
- `name` (**required**) - The screenshot name; must be unique to each screenshot
- `options` (**optional**) - There are various options supported by percy_screenshot to server further functionality.
    - `freezeAnimatedImage` - Boolean value by default it falls back to `false`, you can pass `true` and percy will freeze image based animations.
    - `freezeImageBySelectors` - List of selectors. Images will be freezed which are passed using selectors. For this to work `freezeAnimatedImage` must be set to true.
    - `freezeImageByXpaths` - List of xpaths. Images will be freezed which are passed using xpaths. For this to work `freezeAnimatedImage` must be set to true.
    - `percyCSS` - Custom CSS to be added to DOM before the screenshot being taken. Note: This gets removed once the screenshot is taken.
    - `ignoreRegionXpaths` - List of xpaths. elements in the DOM can be ignored using xpath
    - `ignoreRegionSelectors` - List of selectors. elements in the DOM can be ignored using selectors.
    - `ignoreRegionAppiumElements` - List of appium web-element. elements can be ignored using appium_elements.
    - `customIgnoreRegions` - List of custom objects. elements can be ignored using custom boundaries. Just passing a simple object for it like below.
      - Refer to example -
        - ```
          List<object> ignoreCustomElement = new List<object>();
          var region1 = new Dictionary<string, int>();
          region1.Add("top", 10);
          region1.Add("bottom", 120);
          region1.Add("right", 10);
          region1.Add("left", 10);
          ignoreCustomElement.Add(region1);
          region1.Add("custom_ignore_regions", ignoreCustomElement);
          ```
    - `considerRegionXpaths` - List of xpaths. elements in the DOM can be considered for diffing and will be ignored by Intelli Ignore using xpaths.
    - `considerRegionSelectors` - List of selectors. elements in the DOM can be considered for diffing and will be ignored by Intelli Ignore using selectors.
    - `considerRegionAppiumElements` - List of appium web-element. elements can be considered for diffing and will be ignored by Intelli Ignore using appium_elements.
    - `customConsiderRegions` - List of custom objects. elements can be considered for diffing and will be ignored by Intelli Ignore using custom boundaries
      - Refer to example -
        - ```
          List<object> considerCustomElement = new List<object>();
          var region2 = new Dictionary<string, int>();
          region2.Add("top", 10);
          region2.Add("bottom", 120);
          region2.Add("right", 10);
          region2.Add("left", 10);
          considerCustomElement.Add(region2);
          region2.Add("custom_consider_regions", considerCustomElement);
          ```
        - Parameters:
          - `top` (int): Top coordinate of the consider region.
          - `bottom` (int): Bottom coordinate of the consider region.
          - `left` (int): Left coordinate of the consider region.
          - `right` (int): Right coordinate of the consider region.

### Creating Percy on automate build
Note: Automate Percy Token starts with `auto` keyword. The command can be triggered using `exec` keyword.
```sh-session
$ export PERCY_TOKEN=[your-project-token]
$ percy exec -- [python test command]
[percy] Percy has started!
[percy] [Python example] : Starting automate screenshot ...
[percy] Screenshot taken "Python example"
[percy] Stopping percy...
[percy] Finalized build #1: https://percy.io/[your-project]
[percy] Done!
```

Refer to docs here: [Percy on Automate](https://docs.percy.io/docs/integrate-functional-testing-with-visual-testing)
