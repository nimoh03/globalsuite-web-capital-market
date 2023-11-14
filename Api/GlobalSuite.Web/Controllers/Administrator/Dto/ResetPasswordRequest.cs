using System.ComponentModel.DataAnnotations;

namespace GlobalSuite.Web.Controllers.Dto
{
    public class ResetPasswordRequest
    {
        [Required]
        public string Username { get; set; }
        [Required, DataType(DataType.Password)]
        public string Password { get; set; }
        [Required, DataType(DataType.Password), Compare("Password", ErrorMessage ="Password does not match")]
        public string ConfirmPassword { get; set; }
    }
}
