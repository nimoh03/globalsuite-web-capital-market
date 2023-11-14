using System;

namespace GlobalSuite.Core.Exceptions
{
    public class AppException : Exception
    {
        public AppException() : base("Internal Server Error")
        {
        }

        public AppException(string message, Exception innerException = null) : base(message)
        {
        }

    }
}