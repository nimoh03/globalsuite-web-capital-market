using System;
using System.ComponentModel.DataAnnotations;

namespace GlobalSuite.Web.Controllers.Dto
{
    public class ClosedPeriodRequest
    {
        [Required]
        public DateTime MonthDate { get; set; }
        [Required] public DateTime YearStartDate { get; set; }
        [Required] public DateTime YearEndDate { get; set; }
    }
}