using System;
using PercyIO.Appium;
using Xunit;

namespace Percy.Tests
{
  public class ReflectionUtilsTests
  {
    [Fact]
    public void TestPropertyCall()
    {
      // Arrange
      var obj = new TestDummyClass { Name = "John Doe", Age = 30 };

      // Act
      var result = ReflectionUtils.PropertyCall<String>(obj, "Name");

      // Assert
      Assert.Equal("John Doe", result);
    }

    [Fact]
    public void TestPropertyCall_ReturnDefaultValueForInvalidProperty()
    {
      // Arrange
      var obj = new TestDummyClass { Name = "John Doe", Age = 30 };

      // Act
      var result = ReflectionUtils.PropertyCall<String>(obj, "Invalid");

      // Assert
      Assert.Null(result);
    }

    [Fact]
    public void TestMethodCall()
    {
      // Arrange
      var obj = new TestDummyClass { Name = "John Doe", Age = 30 };

      // Act
      var result = ReflectionUtils.MethodCall<String>(obj, "getName");

      // Assert
      Assert.Equal("John Doe", result);
    }

    [Fact]
    public void TestMethodCall_ReturnDefaultValueForInvalidProperty()
    {
      // Arrange
      var obj = new TestDummyClass { Name = "John Doe", Age = 30 };

      // Act
      var result = ReflectionUtils.MethodCall<String>(obj, "Invalid");

      // Assert
      Assert.Null(result);
    }

    [Fact]
    public void TestMethodCall_ThrowsException()
    {
      // Arrange
      var obj = new TestDummyClass { Name = "John Doe", Age = 30 };

      // Assert
      // call method that is overloaded
      Assert.Throws<PercyException>(() => ReflectionUtils.MethodCall<String>(obj, "setName"));
    }

    // Custom dynamic object class
    public class TestDummyClass
    {
      public string Name { get; set; }
      public int Age { get; set; }

      public void setName(String name)
      {
        this.Name = name;
      }

      public void setName(String firstName, String lastName)
      {
        this.Name = firstName + " "+ lastName;
      }

      public String getName()
      {
        return this.Name;
      }
    }
  }
}