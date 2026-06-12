using System;
using Xunit;

namespace Percy.Tests
{
  public class PercyExceptionTests
  {
    [Fact]
    public void Constructor_WithMessage_SetsMessage()
    {
      // Act
      var exception = new PercyException("something went wrong");

      // Assert
      Assert.Equal("something went wrong", exception.Message);
      Assert.Null(exception.InnerException);
    }

    [Fact]
    public void Constructor_WithMessageAndInnerException_SetsBoth()
    {
      // Arrange
      var inner = new Exception("inner cause");

      // Act
      var exception = new PercyException("outer message", inner);

      // Assert
      Assert.Equal("outer message", exception.Message);
      Assert.Same(inner, exception.InnerException);
    }
  }
}
