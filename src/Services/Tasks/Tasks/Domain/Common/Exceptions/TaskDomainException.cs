namespace Tasks.Domain.Common.Exceptions;

public class TaskDomainException : Exception
{
    public TaskDomainException()
    { }

    public TaskDomainException(string message)
        : base(message)
    { }

    public TaskDomainException(string message, Exception innerException)
        : base(message, innerException)
    { }
}
