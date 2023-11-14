using System;
using GlobalSuite.Core.Admin.Models;
using GlobalSuite.Core.Customers.Models;

namespace GlobalSuite.Core.Accounting.Models
{
    public class PaymentResponse:StatusResponse
    {
        public string PaymentNo { get; set; }
        public string AcctMasBank { get; set; }
        public string AcctSubBank { get; set; }
        public DateTime RNDate { get; set; }
        public string TransDesc { get; set; }
        public decimal Amount { get; set; }
        public string Ref { get; set; }
        public string ChqueNo { get; set; }
        public string Custno { get; set; }
        public string Amtword { get; set; }
        public string PVno { get; set; }
        public string Branch { get; set; }
        public string InstrumentType { get; set; }
        public int InstrumentTypeId { get; set; }
        public PublicCustomer Customer { get; set; }
        public ProductDetailResponse AccoutMasBank { get; set; }
        public BranchResponse BranchDetail { get; set; }
        public ChartOfAccountResponse SubBank { get; set; }
    }
}