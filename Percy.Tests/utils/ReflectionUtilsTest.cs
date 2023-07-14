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
    public void TestPropertyCall_ThrowExceptionForInvalidProperty()
    {
      // Arrange
      var obj = new TestDummyClass { Name = "John Doe", Age = 30 };

      // Assert
      Assert.Throws<PercyException>(() => ReflectionUtils.PropertyCall<String>(obj, "Invalid"));
    }

    [Fact]
    public void TestPropertyCall_ThrowExceptionForInvalidPropertyType()
    {
      // Arrange
      var obj = new TestDummyClass { Name = "John Doe", Age = 30 };

      // Assert
      Assert.Throws<PercyException>(() => ReflectionUtils.PropertyCall<Boolean>(obj, "Name"));
    }

    [Fact]
    public void TestPropertyCall_ReturnDefaultValueForNullProperty()
    {
      // Arrange
      var obj = new TestDummyClass { Name = null, Age = 30 };

      // Act
      var result = ReflectionUtils.PropertyCall<String>(obj, "Name");

      // Assert
      Assert.Equal(null, result);
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
    public void TestMethodCall_ThrowExceptionForInvalidMethod()
    {
      // Arrange
      var obj = new TestDummyClass { Name = "John Doe", Age = 30 };

      // Assert
      Assert.Throws<PercyException>(() => ReflectionUtils.MethodCall<String>(obj, "Invalid"));
    }

    [Fact]
    public void TestMethodCall_ThrowExceptionForInvalidMethodReturnType()
    {
      // Arrange
      var obj = new TestDummyClass { Name = "John Doe", Age = 30 };

      // Assert
      Assert.Throws<PercyException>(() => ReflectionUtils.MethodCall<Boolean>(obj, "getName"));
    }

    [Fact]
    public void TestMethodCall_ReturnDefaultValueForNullMethodReturn()
    {
      // Arrange
      var obj = new TestDummyClass { Name = null, Age = 30 };

      // Act
      var result = ReflectionUtils.MethodCall<String>(obj, "getName");

      // Assert
      Assert.Equal(null, result);
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