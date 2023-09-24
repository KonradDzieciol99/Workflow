using System;

namespace HttpMessage.Models.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string message)
        : base(message) { }
}
