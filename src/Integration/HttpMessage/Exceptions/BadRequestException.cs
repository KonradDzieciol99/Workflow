using System;

namespace HttpMessage.Exceptions;

public class BadRequestException : Exception
{
    public BadRequestException(string message)
        : base(message) { }
}
