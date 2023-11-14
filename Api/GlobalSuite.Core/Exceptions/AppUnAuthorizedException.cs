namespace GlobalSuite.Core.Exceptions
{
    public class AppUnAuthorizedException : AppException
    {
        public AppUnAuthorizedException() : base("User not LoggedIn")
        {
        }
    }
}