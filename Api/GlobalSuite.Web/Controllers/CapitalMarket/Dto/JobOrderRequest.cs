using System;
using System.ComponentModel.DataAnnotations;

namespace GlobalSuite.Web.Controllers.CapitalMarket.Dto
{
    public class JobOrderRequest
    {
        [Required]  public string Code { get; set; }
        [Required]  public string CustNo { get; set; }
        [Required]  public DateTime EffectiveDate { get; set; }
        [Required]  public DateTime DateLimit { get; set; } 
        [Required]  public string StockCode { get; set; }
        [Required]  public int Units { get; set; }
        [Required]  public int AmtProc { get; set; }
        [Required]  public int AmtCanBuy { get; set; }
        [Required]  public decimal UnitPrice { get; set; }
        [Required]  public decimal Amount { get; set; }
        [Required]  public decimal PriceLimit { get; set; }
        [Required]  public decimal OutStandAmount { get; set; }
        [Required]  public decimal CustCreditLimit { get; set; }
        [Required]  public string Instructions { get; set; }
        [Required]  public int TxnType { get; set; }
        [Required]  public string CustNo_CD { get; set; }
        [Required]  public string JB_ID { get; set; }
        [Required]  public string Port_ID { get; set; }
        [Required]  public string MAcct { get; set; }
        [Required]  public string AgAcct { get; set; }
        [Required]  public string Broker { get; set; }
        [Required]  public string TransNoRev { get; set; }
        
    }
}