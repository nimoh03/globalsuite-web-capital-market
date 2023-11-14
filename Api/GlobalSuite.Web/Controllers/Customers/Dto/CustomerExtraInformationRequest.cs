using System;

namespace GlobalSuite.Web.Controllers.Customers.Dto
{
    public class CustomerExtraInformationRequest
    {
        public string CustAID { get; set; }
        public string Gender { get; set; }
        public string Religion { get; set; }
        public string MaritalStatus { get; set; }
        public string SpouseFullName { get; set; }
        public string SpouseEmailAddress { get; set; }
        public string SMSPhone { get; set; }
        public string Website { get; set; }
        public string MailingAddress1 { get; set; }
        public string MailingAddress2 { get; set; }
        public DateTime WedAnniversaryDate { get; set; }
        public string NIMCID { get; set; }
        public string RiskTolerance { get; set; }
        public string DurationOfInvestment { get; set; }
        public string OnlineRegistration { get; set; }
        public bool PEP { get; set; }
        public bool HNI { get; set; }
        public bool BVNVerified { get; set; }
        public bool AddressVerified { get; set; } 
    }
}