namespace Chat.Domain.Common.Exceptions;

public class ChatDomainException : Exception
{
    public ChatDomainException()
    { }

    public ChatDomainException(string message)
        : base(message)
    { }

    public ChatDomainException(string message, Exception innerException)
        : base(message, innerException)
    { }
}
