using System;

namespace GlobalSuite.Core.Exceptions
{
    public class UserFriendlyException : AppException
    {
        public UserFriendlyException(string message, Exception innerException = null) : base(message)
        {
        }
    }
}