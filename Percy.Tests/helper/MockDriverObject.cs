
using System;
using System.Collections.Generic;

namespace Percy.Tests
{
  internal class MockDriverObject
  {
    private MockCapabilities mockCapabilities;
    public String SessionId { get; set; }
    public MockCapabilities Capabilities
    {
      get
      {
        return mockCapabilities;
      }
    }

    public void SetCapability(Dictionary<string, object> caps)
    {
      mockCapabilities = new MockCapabilities();
      mockCapabilities.Capabilities = caps;
      SessionDetails = new Dictionary<string, object> 
      {
        { "viewportRect",
          new Dictionary<string, object> {
            {"top", 100l},
            {"height", 1000l},
            {"width", 400l},
          }
        },
        {
          "pixelRatio", 1l
        }
      };
    }
    public Dictionary<string, object> SessionDetails { get; set; }

    private Boolean isV5 = false;

    public void setIsV5(Boolean isV5)
    {
      this.isV5 = isV5;
    }

    private CommandExecutor commandExecutor;

    CommandExecutor CommandExecutor
    {
      get
      {
        return commandExecutor;
      }
    }

    public void setCommandExecutor(String url)
    {
      commandExecutor = new CommandExecutor();

      if (isV5)
      {
        commandExecutor.setIsV5(isV5);
      }
      commandExecutor.SetUrl(url);
    }

    public Object GetScreenshot()
    {
      return new Screenshot();
    }

    public Object ExecuteScript(String script)
    {
      return @"{success:'true', osVersion:'11.2', buildHash:'abc', sessionHash:'def'}";
    }
  }

  internal class Screenshot
  {
    public String AsBase64EncodedString
    {
      get
      {
        return "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mNk+A8AAQUBAScY42YAAAAASUVORK5CYII=";
      }
    }
  }
}
