using System;

namespace GlobalSuite.Web.Controllers.CapitalMarket.Dto
{
    public class TradeValuationRequest
    {
         
        public string Txn { get; set; }
        public string CustAid { get; set; }
        public string StockCode { get; set; }
        public DateTime iDate { get; set; }
        public int Qty { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Consideration { get; set; }
        public decimal TotalAmt { get; set; }
        public string CommissionType { get; set; }
        public string TransType { get; set; }
    }
}