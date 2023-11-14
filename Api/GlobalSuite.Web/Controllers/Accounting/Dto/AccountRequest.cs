namespace GlobalSuite.Web.Controllers.Accounting.Dto
{
    public class AccountRequest
    {
        public string TransNo { get; set; }
        public string AccountId { get; set; }
        public string AccountName { get; set; }
        public string AccountType { get; set; }
        public string Branch { get; set; }
        public string ParentId { get; set; }
        public int AccountLevel { get; set; } = 1;
        public string BankAccount { get; set; } = "N";
        public string PettyCashAccount { get; set; }= "N";
        public string BankChargeAccount { get; set; } = "N";
        public int IncomeStateAnnual { get; set; }
        public int SOCFAnnual { get; set; }
        public int SOCIEAnnual { get; set; }
        public int SOFPAnnual { get; set; }
        public string PreviousYearCreditDebitAnnual { get; set; } = string.Empty;
        public bool ExcludeInIFRSReporting { get; set; }
        public bool IsInternal { get; set; }
    }
}