using System;
using CustomerManagement.Business;
using GlobalSuite.Core.Admin.Models;
using GlobalSuite.Core.Customers.Models;

namespace GlobalSuite.Core.Accounting.Models
{
    public class DepositResponse:StatusResponse
    {
        public string Code => ReceiptNo;
        public string ReceiptNo { get; set; }
         public string Custno { get; set; }
         public string RecNo { get; set; }
         public PublicCustomer Customer { get; set; }
         public string Ref { get; set; }
        public string TranDesc { get; set; }
         public decimal Amount { get; set; }
        public DateTime RNDate { get; set; }
        public string AcctSubBank { get; set; }
        public string ChqueNo { get; set; }
        public string AcctMasBank { get; set; }
    
        public string PayDesc { get; set; }
        public bool DoNotChargeBankStampDuty { get; set; }
        public string InstrumentType { get; set; }
        public int InstrumentTypeId { get; set; }
        public string TransNoRev { get; set; }
        public decimal Balance { get; set; }
        public ChartOfAccountResponse SubBank { get; set; }
        public ProductDetailResponse ProductDetail { get; set; }
        public BranchResponse Branch { get; set; }

       
    }
}