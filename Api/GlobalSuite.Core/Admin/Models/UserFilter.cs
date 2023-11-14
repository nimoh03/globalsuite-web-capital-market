namespace GlobalSuite.Core.Admin.Models
{
    public class UserFilter:BaseFilter
    {
        public bool OnlyActive { get; set; }
        public bool OnlySuspended { get; set; }
    }
}