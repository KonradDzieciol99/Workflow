namespace Projects.Domain.Common.Exceptions;

/// <summary>
/// Exception type for domain exceptions
/// </summary>
public class ProjectDomainException : Exception
{
    public ProjectDomainException() { }

    public ProjectDomainException(string message)
        : base(message) { }

    public ProjectDomainException(string message, Exception innerException)
        : base(message, innerException) { }
}
