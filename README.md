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

`percy.screenshot(name, fullScreen)`

- `name` (**required**) - The screenshot name; must be unique to each screenshot
- Additional screenshot options (overrides any project options):
  - `fullScreen ` - It indicates if the app is a full screen
  - `options` - Optional screenshot params:
    Use `ScreenshotOptions` to set following params to override
      - `DeviceName` - Device name on which screenshot is taken
      - `StatusBarHeight` - Height of status bar for the device
      - `NavBarHeight` - Height of navigation bar for the device
      - `Orientation` - Orientation of the application
      - `FullPage`: true/false. [Experimental] only supported on App Automate driver sessions [ needs @percy/cli 1.20.2+ ]
      - `ScreenLengths`: int [Experimental] max screen lengths for fullPage [ needs @percy/cli 1.20.2+ ]
      - `ScrollableXpath` (**optional**) - [Experimental] scrollable element xpath for fullpage [ needs @percy/cli 1.20.2+ ]; string
      - `ScrollableId` (**optional**) - [Experimental] scrollable element accessibility id for fullpage [ needs @percy/cli 1.20.2+ ]; string
      - `IgnoreRegionXpaths` (**optional**) - elements xpaths that user want to ignore in visual diff [ needs @percy/cli 1.23.0+ ]; list of string
      - `IgnoreRegionAccessibilityIds` (**optional**) - elements accessibility_ids that user want to ignore in visual diff [ needs @percy/cli 1.23.0+ ]; list of string
      - `IgnoreRegionAppiumElements` (**optional**) - appium elements that user want to ignore in visual diff [ needs @percy/cli 1.23.0+ ]; list of appium element object
      - `CustomIgnoreRegions` (**optional**) - custom locations that user want to ignore in visual diff [ needs @percy/cli 1.23.0+ ]; list of ignore_region object
      - IgnoreRegion:-
        - Description: This class represents a rectangular area on a screen that needs to be ignored for visual diff.
        - constructor:-
          ```
          var ignoreRegion = new IgnoreRegion();
          ignoreRegion.Top = top;
          ignoreRegion.Bottom = bottom;
          ignoreRegion.Left = left;
          ignoreRegion.Right = right;
          ```
        - Parameters:

          `Top` (int): Top coordinate of the ignore region.

          `Bottom` (int): Bottom coordinate of the ignore region.

          `Left` (int): Left coordinate of the ignore region.

          `Right` (int): Right coordinate of the ignore region.
        - Raises:ArgumentException: If top, bottom, left, or right is less than 0 or top is greater than or equal to bottom or left is greater than or equal to right.
        - valid: Ignore region should be within the boundaries of the screen.

### Migrating Config

If you have a previous Percy configuration file, migrate it to the newest version with the
[`config:migrate`](https://github.com/percy/cli/tree/master/packages/cli-config#percy-configmigrate-filepath-output) command:

```sh-session
$ percy config:migrate
```
