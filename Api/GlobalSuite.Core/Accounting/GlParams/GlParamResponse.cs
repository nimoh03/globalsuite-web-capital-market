using GlobalSuite.Core.Accounting.Models;

namespace GlobalSuite.Core.Accounting
{
    public class GlParamResponse
    {
        public ProductResponse CustomerProductDetail { get; set; }
        public ProductResponse VendorProductDetail { get; set; }
        public ChartOfAccountResponse PettyCashAccount { get; set; }
        public ChartOfAccountResponse PayableAccount { get; set; } 
        public ChartOfAccountResponse PurchaseAccount { get; set; }  
        public ChartOfAccountResponse PurchaseAccountIncome { get; set; }  
        public ChartOfAccountResponse FXAssetControlAccount  { get; set; }
        public ChartOfAccountResponse GLOpenAccount { get; set; }
        public ChartOfAccountResponse COTAccount { get; set; }
        public ChartOfAccountResponse VATAccount { get; set; }
        public ChartOfAccountResponse SMSChargeAccount { get; set; }
        public ChartOfAccountResponse SMSAlertIncomeAccount { get; set; }
        public ChartOfAccountResponse ReserveAccount { get; set; }
        public ChartOfAccountResponse CustOpenAccount { get; set; }
        public ChartOfAccountResponse SalesAccount { get; set; }
        public ChartOfAccountResponse RevaluationReserveAccount { get; set; }
        public ChartOfAccountResponse IncomeAccountForBankStampDuty { get; set; }
        public ChartOfAccountResponse SalesAccountIncome { get; set; }
        public decimal COT{ get; set; }
        public float VAT { get; set; }
        public decimal SMSAlert { get; set; }
        public decimal SMSAlertCustomer { get; set; }
        public int BankClearingDay { get; set; }
        public int TradingClearingDay { get; set; }
        public string PayableChargeVAT { get; set; }
        public string ReceivableChargeVAT { get; set; }
        public decimal CBNStampDutyMinimumAmtToCharge { get; set; }
        public decimal CBNStampDutyAmount { get; set; }
    }
}