using System;

namespace GlobalSuite.Web.Controllers.CapitalMarket.Dto
{
    public class AllotmentRequest
    {
        public string CustID { get; set; }
        public string Txn { get; set; }
        public string TransNoRev { get; set; }
        public string StockCode { get; set; }
        public string OldAllotNo { get; set; }
        public DateTime DateAlloted { get; set; }
        public DateTime DateAllotedTo { get; set; }
        public long Qty { get; set; }
        public int NumberOfTrans { get; set; }
        public decimal UnitPrice { get; set; }
        public string CommissionType { get; set; }
        public string Consideration { get; set; }
        public decimal SecFee { get; set; }
        public decimal StampDuty { get; set; }
        public decimal Commission { get; set; }
        public decimal VAT { get; set; }
        public decimal NSEFee { get; set; }
        public decimal CSCSFee { get; set; }
        public decimal SMSAlertCSCS { get; set; }
        public decimal TotalAmount { get; set; } 
        public string CrossType { get; set; }
        public string OtherCust { get; set; }
    }
}