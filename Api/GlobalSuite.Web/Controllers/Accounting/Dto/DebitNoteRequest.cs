using System;
using System.ComponentModel.DataAnnotations;

namespace GlobalSuite.Web.Controllers.Accounting.Dto
{
    public class DebitNoteRequest
    {
        public string Code { get; set; }= string.Empty;
        [Required]  public string CustNo { get; set; }
        public string Ref { get; set; }= string.Empty;
        [Required]  public decimal Amount { get; set; }
        public DateTime RNDate { get; set; } 
        public string AcctSubBank { get; set; }= string.Empty;
        public string AcctMasBank { get; set; }= string.Empty;
        public string Trandesc { get; set; } = string.Empty;
        public bool ChargeVAT { get; set; }
        public bool VATIsInclusive { get; set; }
    }
}