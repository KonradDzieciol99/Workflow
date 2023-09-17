namespace API.Aggregator.Application.Commons.Exceptions;

public class UnauthorizedException : Exception
{
    public UnauthorizedException(string message) : base(message)
    {

    }
    public UnauthorizedException() : base()
    {

    }
}
