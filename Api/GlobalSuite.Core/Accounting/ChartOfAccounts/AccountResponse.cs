using GlobalSuite.Core.Admin.Models;

namespace GlobalSuite.Core.Accounting.Models
{
    public class AccountResponse:ChartOfAccountResponse
    {
        public string TransNo { get; set; }
        public string AccountType { get; set; }
        public string BranchId { get; set; }
        public string ParentId { get; set; }
        public int AccountLevel { get; set; }
        public string BankAccount { get; set; }
        public string PettyCashAccount { get; set; }
        public string BankChargeAccount { get; set; }
        public int IncomeStateAnnual { get; set; }
        public int SOCFAnnual { get; set; }
        public int SOCIEAnnual { get; set; }
        public int SOFPAnnual { get; set; }
        public string PreviousYearCreditDebitAnnual { get; set; } = "N";
        public bool ExcludeInIFRSReporting { get; set; }
        public bool IsParent { get; set; }
        public bool IsInternal { get; set; }
        public ChartOfAccountResponse Parent { get; set; }
        public BranchResponse BranchDetail { get; set; }
    }
}