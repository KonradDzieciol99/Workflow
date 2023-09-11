namespace Tasks.Application.Common.Exceptions;

public class ForbiddenAccessException : Exception
{
    public ForbiddenAccessException() : base("You do not have access to this resource") { }
}
