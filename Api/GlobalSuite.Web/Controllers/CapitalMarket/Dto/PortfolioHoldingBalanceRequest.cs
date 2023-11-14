using System;
using System.ComponentModel.DataAnnotations;

namespace GlobalSuite.Web.Controllers.CapitalMarket.Dto
{
    public class PortfolioHoldingBalanceRequest
    {
      [Required]  public string TransNo { get; set; }
        public string Product { get; set; }
     [Required]   public string CustNo { get; set; }
        public DateTime EffDate { get; set; }
        public string Stockcode { get; set; }
        public int Units { get; set; }
        public float UnitPrice { get; set; }
        public decimal TotalAmount { get; set; }
        public string TransNoRev { get; set; }
    }
}