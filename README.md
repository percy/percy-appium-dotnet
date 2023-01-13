# percy-appium-dotnet
![Test](https://github.com/percy/percy-appium-dotnet/workflows/Test/badge.svg)

[Percy](https://percy.io) visual testing for .NET Selenium.

## Development

Install/update `@percy/cli` dev dependency (requires Node 14+):

```sh-session
$ npm install --save-dev @percy/cli
```

Install dotnet SDK:

```sh-session
$ brew tap isen-ng/dotnet-sdk-versions
$ brew install --cask  dotnet-sdk5-0-400
$ dotnet --list-sdks
```

Install Mono:

```sh-session
$ brew install mono
$ mono --version 
```

Run tests:

```
npm run test
```
## NuGet

NuGet Package: [](http://www.nuget.org/packages/PercyIO.Appium/)

Dependencies:

- [Appium.WebDriver](https://www.nuget.org/packages/Appium.WebDriver/4.0.0) Version >= 4.0 and < 5
- [Newtonsoft.Json](http://www.nuget.org/packages/Newtonsoft.Json/) Version >= 12.0.1
- [Castle.Core](https://www.nuget.org/packages/Castle.Core/) Version >= 4.3.1
- [Microsoft.CSharp](https://www.nuget.org/packages/Microsoft.CSharp) Version >= 4.7.0
- [DotNetSeleniumExtras.WaitHelpers](https://www.nuget.org/packages/DotNetSeleniumExtras.WaitHelpers)


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
using PercyIO.Appium;

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
      - `deviceName` - Device name on which screenshot is taken
      - `statusBarHeight` - Height of status bar for the device
      - `navBarHeight` - Height of navigation bar for the device
      - `orientation`  - Orientation of the application

### Migrating Config

If you have a previous Percy configuration file, migrate it to the newest version with the
[`config:migrate`](https://github.com/percy/cli/tree/master/packages/cli-config#percy-configmigrate-filepath-output) command:

```sh-session
$ percy config:migrate
```
