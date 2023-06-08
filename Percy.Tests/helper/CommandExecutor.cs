using System;

namespace Percy.Tests
{
  public class CommandExecutor
  {
    private Uri URL;
    private Boolean isV5 = false;
    private RealExecutor RealExecutor;
    public void SetUrl(String url)
    {
      if (isV5)
      {
        RealExecutor = new RealExecutor();
        RealExecutor.SetUrl(url);
      }
      else
      {
        URL = new Uri(url);
      }
    }
  
    public void setIsV5(Boolean isV5)
    {
      this.isV5 = isV5;
    }
  }

  public class RealExecutor
  {
    private Uri remoteServerUri;
    public void SetUrl(String url)
    {
      remoteServerUri = new Uri(url);;
    }
  }
}
