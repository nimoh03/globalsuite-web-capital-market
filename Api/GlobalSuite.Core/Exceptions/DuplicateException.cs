namespace GlobalSuite.Core.Exceptions
{
    public class DuplicateException : AppException
    {
        public DuplicateException(string message) : base(message)
        {
        }
    }
}