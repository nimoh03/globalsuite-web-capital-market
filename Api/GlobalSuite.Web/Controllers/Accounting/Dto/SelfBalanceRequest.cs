using System;
using System.ComponentModel.DataAnnotations;

namespace GlobalSuite.Web.Controllers.Accounting.Dto
{
    public class SelfBalanceRequest
    {
        public string TransNo { get; set; } = string.Empty;
       [Required(ErrorMessage = "Main Account")] 
       public string MainAcctID { get; set; }
       [Required(ErrorMessage = "Con Account")] 
        public string ConAccountID { get; set; }
        // public DateTime TxnDate { get; set; }
       [Required(ErrorMessage = "Desciption is Required.")] 
       public string Description { get; set; }
       public string Ref { get; set; } = string.Empty;
        [Required(ErrorMessage = "Amount is Required")] 
        public decimal Amount { get; set; }
        // public string MainTransType { get; set; } = string.Empty;
        public int InstrumentType { get; set; }
        // [Required(ErrorMessage = "VDate is required")]
        // public DateTime VDate { get; set; }
    }
}