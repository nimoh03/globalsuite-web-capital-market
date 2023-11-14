using System;
using GlobalSuite.Core.Admin.Models;
using GlobalSuite.Core.Customers.Models;

namespace GlobalSuite.Core.Accounting.Models
{
    public class OpeningBalanceResponse:StatusResponse
    {
         public string TransNo { get; set; }
         public string CustNo { get; set; }
         public PublicCustomer Customer { get; set; }
         public string Ref { get; set; }
         public string BranchId { get; set; }
         public BranchResponse BranchDetail { get; set; }
         public string DebCred { get; set; }
        public string TranDesc { get; set; }
         public decimal Amount { get; set; }
        public DateTime RNDate { get; set; }
    }
}