using System;
using System.ComponentModel.DataAnnotations;

namespace GlobalSuite.Web.Controllers.CapitalMarket.Dto
{
    public class PortfolioHoldingInternalRequest
    {
      [Required]  public string TransNo { get; set; }
     [Required]   public string CustomerId { get; set; }
        public DateTime EffDate { get; set; }
        public string StockCode { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public float UnitPrice { get; set; }
    }
}