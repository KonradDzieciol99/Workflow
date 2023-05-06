using System;
using System.Collections.Generic;
using System.Text;

namespace HttpMessage.Models.Exceptions
{
    public class ForbiddenException : Exception
    {
        public ForbiddenException(string message) : base(message)
        {
            
        }
    }
}
