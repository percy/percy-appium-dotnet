using System;

public class PercyException : Exception
{
    public PercyException(string message) : base(message)
    {
    }

    public PercyException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
