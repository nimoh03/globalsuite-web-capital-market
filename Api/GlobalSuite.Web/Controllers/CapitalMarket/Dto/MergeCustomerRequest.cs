using System.Collections.Generic;

namespace GlobalSuite.Web.Controllers.CapitalMarket.Dto
{
    public class MergeCustomerRequest
    {
        public string TransNo { get; set; }
        public string AcctID { get; set; }
        public List<string> CustomerAccounts { get; set; } = new List<string>();
    }
}