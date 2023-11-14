namespace GlobalSuite.Web.Controllers.Accounting.Dto
{
    public class GlParamRequest
    {
        public string CustomerProduct { get; set; }
        public string VendorProduct { get; set; }
        public string PettyCashAcct { get; set; }
        public string PayableAcct { get; set; } = string.Empty;
        public string FXAssetControlAcct { get; set; } = string.Empty;
        public string SalesAcct { get; set; }
        public string SalesAcctIncome { get; set; }
        public string PurchaseAcct { get; set; } = string.Empty;
        public string PurchaseAcctIncome { get; set; } = string.Empty;
        public string GLOpenAcct { get; set; }
        public string COTAcct { get; set; }
        public string VATAcct { get; set; }
        public string SMSChargeAcct { get; set; }
        public string SMSAlertIncomeAcct { get; set; }
        public string ReserveAcct { get; set; }
        public string CustOpenAcct { get; set; }
        public decimal COT{ get; set; }
        public float VAT { get; set; }
        public decimal SMSAlert { get; set; }
        public decimal SMSAlertCustomer { get; set; }
        public string PayableChargeVAT { get; set; }
        public string ReceivableChargeVAT { get; set; }
        public int BankClearingDay { get; set; }
        public int TradingClearingDay { get; set; }
    }
}