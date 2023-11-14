using System.ComponentModel.DataAnnotations;

namespace GlobalSuite.Web.Controllers.Dto
{
    public class LgaRequest
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public int StateId { get; set; }
    }
}