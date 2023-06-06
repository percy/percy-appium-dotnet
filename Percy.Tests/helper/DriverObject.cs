
using System;
using System.Collections.Generic;
using System.Dynamic;

namespace Percy.Tests
{
  internal class DriverObject
  {
    public dynamic CreateDynamicAppiumDriver(String os)
    {
        dynamic dynamicObject = new ExpandoObject();

        // Set properties
        dynamicObject.SessionId = "12345";
        // dynamicObject.Capabilities = new DesiredCapabilities();

        dynamicObject.Capabilities = new {
          capabilities = MetadataBuilder.CapabilityBuilder(os)
        };

        dynamicObject.SessionDetails = new Dictionary<string, object> {
          { "viewportRect", 
            new Dictionary<string, object> {
              {"top", 100l},
              {"height", 1000l},
              {"width", 400l},
            }
         },
         {
          "pixelRatio", 1
         }
        };
        
        // Define methods
        dynamicObject.FindElement = new Func<string, object>((locator) =>
        {
            // Logic to find element
            return null;
        });


        // Add more properties and methods as needed

        return dynamicObject;
    } 
  }
}