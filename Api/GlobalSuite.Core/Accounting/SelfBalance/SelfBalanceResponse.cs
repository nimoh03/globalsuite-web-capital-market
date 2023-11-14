using System;
using GlobalSuite.Core.Accounting.Models;
using GlobalSuite.Core.Admin.Models;

namespace GlobalSuite.Core.Accounting
{
    public class SelfBalanceResponse:StatusResponse
    {
        public string TransNo { get; set; }
        public string MainAcctID { get; set; }
        public string ConAcctID { get; set; }
        public DateTime VDate { get; set; }
        public DateTime TxnDate { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string MasTransType { get; set; }
        public string UserID { get; set; }
        public string Branch { get; set; }
        public string ChqDate { get; set; }
        public string InstrumentType { get; set; }

        public ChartOfAccountResponse MainAcct { get; set; }
        public ChartOfAccountResponse ConAcct { get; set; }
        public BranchResponse BranchDetail { get; set; }
    }
}