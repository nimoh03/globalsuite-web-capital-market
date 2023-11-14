using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GlobalSuite.Web.Controllers.Accounting.Dto
{
    public class BatchPosting
    {
        public DateTime EffectiveDate { get; set; }
        public List<BatchPostingRequest> Transactions { get; set; } = new List<BatchPostingRequest>();
    }
    public class BatchPostingRequest
    {
        public string AccountId { get; set; }
        public string Description { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
    }
    
   
}