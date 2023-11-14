using System;
using System.ComponentModel.DataAnnotations;

namespace GlobalSuite.Web.Controllers.Accounting.Dto
{
    public class PaymentRequest
    {
        [Required]
        public string AcctSubBank { get; set; }
        [Required]
        public string Ref { get; set; }
        [Required]  
        public decimal Amount { get; set; }

        public string ChequeNo { get; set; } = string.Empty;
        public string TransDesc { get; set; }= string.Empty;
        [Required]  
        public string CustNo { get; set; }
        public string AcctMasBank { get; set; }
        public int InstrumentType { get; set; }
        public DateTime RNDate { get; set; }
        public bool DoNotChargeBankStampDuty { get; set; }  
        
        
    }

    public class PaymentUpdateRequest : PaymentRequest
    {
        [Required(ErrorMessage = "Code is required")]
        public string Code { get; set; }
    }
}