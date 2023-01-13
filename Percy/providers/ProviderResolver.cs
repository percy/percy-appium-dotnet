namespace PercyIO.Appium
{
  internal class ProviderResolver
  {

    internal static GenericProvider ResolveProvider(IPercyAppiumDriver percyAppiumDriver)
    {
      if (AppAutomate.Supports(percyAppiumDriver))
      {
        return new AppAutomate(percyAppiumDriver);
      }
      else
      {
        return new GenericProvider(percyAppiumDriver);
      }
    }
  }
}
