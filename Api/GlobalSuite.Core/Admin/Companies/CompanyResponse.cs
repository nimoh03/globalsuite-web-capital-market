using System;

namespace GlobalSuite.Core.Admin
{
    public class CompanyResponse
    {
        public long TransNo { get; set; }
        public string Coy_Name { get; set; }
        internal string Address_1 { get; set; }
        public string Member_Code { get; set; }
        public string Address_2 { get; set; }
        public string Town { get; set; }
        public string State { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public DateTime Current_Date_of_Processing { get; set; }
        public DateTime Last_Date_of_Processing { get; set; }
        public DateTime Dep_Date { get; set; }
        public DateTime CustBalDate { get; set; }
        public DateTime CustBalDateFrom { get; set; }
        public DateTime PortfolioDate { get; set; }
        public DateTime EODRunDate { get; set; }
        public DateTime EOMRunDate { get; set; }
        public DateTime CustStmtTo { get; set; }
        public DateTime CustStmtFrom { get; set; }
        public DateTime AcctStmtTo { get; set; }
        public DateTime AcctStmtFrom { get; set; }
        public string CustStmtCustId { get; set; }
        public string AcctStmtAccountId { get; set; }
        public string Fax { get; set; }
        public string Web { get; set; }
        public string DefaultBranch { get; set; }
        public string Branch { get; set; }
        public string VATNo { get; set; }
        public string RCNo { get; set; }
        public string SMSDefaultPhoneNo { get; set; }
        public string SMSSenderID { get; set; }
        public string SMSDefaultText { get; set; }
        public string SMSUserName { get; set; }
        public string ErrMessageToReturn { get; set; }

       
    }
}