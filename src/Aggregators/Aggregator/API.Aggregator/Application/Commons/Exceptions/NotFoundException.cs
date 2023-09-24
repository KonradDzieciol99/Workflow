namespace API.Aggregator.Application.Commons.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string message)
        : base(message) { }

    public NotFoundException()
        : base() { }
}
