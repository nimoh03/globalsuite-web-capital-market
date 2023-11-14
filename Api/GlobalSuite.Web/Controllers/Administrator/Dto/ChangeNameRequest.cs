using System.ComponentModel.DataAnnotations;

namespace GlobalSuite.Web.Controllers.Dto
{
    public class ChangeNameRequest
    {
        [Required]
        public string FullName { get; set; }
        [Required]
        public string EmailAddress { get; set; }
        [Required]
        public string BranchId { get; set; }
    }
}