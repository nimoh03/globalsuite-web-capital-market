using System;
using GlobalSuite.Core.Admin.Models;
using GlobalSuite.Core.Customers.Models;

namespace GlobalSuite.Core.Accounting.Models
{
    public class TransferResponse : StatusResponse
    {
        public string TransNo { get; set; }
        public string TProduct { get; set; }
        public string RProduct { get; set; }
        public string RCustAID { get; set; }
        public DateTime EffDate { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string Ref { get; set; }
        public string TCustAID { get; set; }
        public string Branch { get; set; }
        public string UserId { get; set; }
        public DateTime TxnDate { get; set; }
        public DateTime TxnTime { get; set; }
        public PublicCustomer  RCustomer{ get; set; }
        public PublicCustomer  TCustomer{ get; set; }
        public ProductDetailResponse FromProductDetail { get; set; }
        public ProductDetailResponse ToProductDetail { get; set; }
        public BranchResponse BranchDetail { get; set; }
    }
}