using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace GlobalSuite.Web.Controllers.Accounting.Dto
{
    public class ProductClassResponse
    {
        public string ProductClassCode { get; set; }
        public string ProductClassName { get; set; }
        public string Module { get; set; }
    }
}