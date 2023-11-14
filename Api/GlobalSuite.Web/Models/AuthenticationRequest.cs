using System.ComponentModel.DataAnnotations;

namespace GlobalSuite.Web.Models
{
    public class AuthenticationRequest
    {
        [Required]
        public string Username { get; set; }
        [Required, DataType(DataType.Password)]
        public string Password { get; set; }
    }

    public class AuthenticationResponse
    {
        public string Token { get; set; }
    }
    public class AuthenticationErrorResponse
    {
        public AuthenticationErrorResponse(string error, bool canLogin=false, bool requirePasswordChange=false)
        {
            Error = error;
            CanLogin = canLogin;
            RequirePasswordChange = requirePasswordChange;
        }
        public string Error { get; set; }
        public bool CanLogin { get; set; }
        public bool RequirePasswordChange { get; set; }
    }
}