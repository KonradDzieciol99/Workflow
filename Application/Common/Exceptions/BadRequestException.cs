using System.Runtime.Serialization;

namespace WorkflowApi.Exceptions
{

    public class BadRequestException : Exception
    {
        public BadRequestException(string message) : base(message)
        {
        }
    }
}