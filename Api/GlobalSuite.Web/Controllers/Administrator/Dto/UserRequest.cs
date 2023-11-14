namespace GlobalSuite.Web.Controllers.Dto
{
    public class UserRequest:ChangeNameRequest
    {
        public string UserNameAccount { get; set; }
        public string Password { get; set; }
        public string Group { get; set; }
    }

    public class UserResponse : ChangeNameRequest
    {
    }
}