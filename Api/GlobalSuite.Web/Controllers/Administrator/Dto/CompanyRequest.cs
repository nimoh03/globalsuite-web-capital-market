using System;
using System.ComponentModel.DataAnnotations;

namespace GlobalSuite.Web.Controllers.Dto
{
    public class CompanyRequest
    {
       
        [Required]
         public string CompanyName { get; set; }
        public string Address1 { get; set; } = "";
        public string Address2 { get; set; } = "";
        public string Town { get; set; } = "";  
        public string State { get; set; } = "";
        public string Phone { get; set; } = "";
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = "";     
        public string Fax { get; set; } = "";
        [DataType(DataType.Url)]
        public string Web { get; set; } = "";
    [Required]    public DateTime StartYear { get; set; }
        [Required] public DateTime EndYear { get; set; }
        public string VATNo { get; set; } = "";
        public string RCNo { get; set; } = "";
        public string SMSDefaultPhoneNo { get; set; } = "";
        public string SMSSenderID { get; set; } = "";
        public string SMSDefaultText { get; set; } = "";
        public string SMSUserName { get; set; } = "";
        public string SMSUserPassword { get; set; } = "";
        public DateTime EOMRunDate { get; set; } = DateTime.MinValue;
        public DateTime EODRunDate { get; set; } = DateTime.MinValue;
        public int PasswordValidityDay { get; set; } = 0;
        public int DormantValidityDay { get; set; }= 0; 
        public int EndYearDeadlineExtension { get; set; } = 0;
        public int UtilityBillMonthAllowance { get; set; } = 0;
        public string CompanyPhoto { get; set; } = "";
        public string CompanyLogo { get; set; } = "";
     }
}