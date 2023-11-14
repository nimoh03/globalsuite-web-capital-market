using System;

namespace GlobalSuite.Web.Controllers.CapitalMarket.Dto
{
    public class FixPullOutRequest
    {
        public DateTime TradeSummary { get; set; }
        public DateTime AccountSummary { get; set; }
        public DateTime Portfolio { get; set; }
    }
}