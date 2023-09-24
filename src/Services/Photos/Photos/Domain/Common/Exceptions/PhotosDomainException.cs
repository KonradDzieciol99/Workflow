namespace Photos.Domain.Common.Exceptions;

public class PhotosDomainException : Exception
{
    public PhotosDomainException() { }

    public PhotosDomainException(string message)
        : base(message) { }

    public PhotosDomainException(string message, Exception innerException)
        : base(message, innerException) { }
}
