namespace PercyIO.Appium
{
  internal interface IPercyAppiumCapabilities
  {
    T getValue<T>(string key);
  }
}
