using System;

namespace GlobalSuite.Web.Controllers.Customers.Dto
{
    public class CustomerBankRequest
    {
        public string CustAID { get; set; }
        public long BankId { get; set; }
        public string BankName { get; set; } = "";
        public string BankAccountNo { get; set; } = "";
        public DateTime BankDateOpen { get; set; }
        public string BankAddress { get; set; } = "";
    }
}