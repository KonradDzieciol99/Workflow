namespace API.Aggregator.Domain.Commons.Exceptions;

public class AggregatorDomainException : Exception
{
    public AggregatorDomainException() { }

    public AggregatorDomainException(string message)
        : base(message) { }

    public AggregatorDomainException(string message, Exception innerException)
        : base(message, innerException) { }
}
