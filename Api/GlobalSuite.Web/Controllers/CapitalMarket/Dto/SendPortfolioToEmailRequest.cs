using System;
using System.ComponentModel.DataAnnotations;

namespace GlobalSuite.Web.Controllers.CapitalMarket.Dto
{
    public class SendPortfolioToEmailCsCsRequest
    {
     [Required]   public string TransNo { get; set; }
     [Required]   public DateTime UploadDate { get; set; }
    }
}