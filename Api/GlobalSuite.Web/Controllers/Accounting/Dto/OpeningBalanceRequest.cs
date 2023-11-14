using System;
using System.ComponentModel.DataAnnotations;

namespace GlobalSuite.Web.Controllers.Accounting.Dto
{
    public class OpeningBalanceRequest
    {
        [Required]  public string Code { get; set; }
        [Required]  public string CustNo { get; set; }
        [Required]  public string Ref { get; set; }
        public string BranchId { get; set; }
        public string TranDesc { get; set; }
        [Required]  public decimal Amount { get; set; }
        public string DebCred { get; set; }
        public DateTime RNDate { get; set; }
    }
}