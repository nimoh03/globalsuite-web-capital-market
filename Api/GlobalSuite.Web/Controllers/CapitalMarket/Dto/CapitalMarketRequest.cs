using System.ComponentModel.DataAnnotations;

namespace GlobalSuite.Web.Controllers.CapitalMarket.Dto
{
    public class CapitalMarketRequest
    {
        [Required]
        public string MemberCode { get; set; }

        public StkParamRequest StkParam { get; set; }
        public PGenTableRequest PGenTable { get; set; }
    }
}