using System;

namespace Percy.Tests
{
  public class CommandExecutor
  {
    private Uri URL;
    private Uri remoteServerUri;
    private Boolean isV5 = false;
    private Boolean useRemoteServerUri = false;
    private Boolean useInternalExecutor = false;
    private Boolean useDirectCommandExecutor = false;
    private RealExecutor RealExecutor;
    private InternalExecutor internalExecutor;

    public void SetUrl(String url)
    {
      if (isV5)
      {
        if (useInternalExecutor)
        {
          // Simulate internalExecutor scenario
          internalExecutor = new InternalExecutor();
          internalExecutor.SetUrl(url);
        }
        else if (useDirectCommandExecutor)
        {
          // Simulate direct commandExecutor with remoteServerUri
          remoteServerUri = new Uri(url);
        }
        else
        {
          // Default V5 behavior with RealExecutor
          RealExecutor = new RealExecutor();
          RealExecutor.SetUrl(url);
        }
      }
      else
      {
        if (useRemoteServerUri)
        {
          remoteServerUri = new Uri(url);
        }
        else
        {
          URL = new Uri(url);
        }
      }
    }

    public void setIsV5(Boolean isV5)
    {
      this.isV5 = isV5;
    }

    public void setUseRemoteServerUri(Boolean useRemoteServerUri)
    {
      this.useRemoteServerUri = useRemoteServerUri;
    }

    public void setUseInternalExecutor(Boolean useInternalExecutor)
    {
      this.useInternalExecutor = useInternalExecutor;
    }

    public void setUseDirectCommandExecutor(Boolean useDirectCommandExecutor)
    {
      this.useDirectCommandExecutor = useDirectCommandExecutor;
    }
  }

  public class RealExecutor
  {
    private Uri remoteServerUri;
    public void SetUrl(String url)
    {
      remoteServerUri = new Uri(url);
    }
  }

  public class InternalExecutor
  {
    private Uri remoteServerUri;
    public void SetUrl(String url)
    {
      remoteServerUri = new Uri(url);
    }
  }
}
