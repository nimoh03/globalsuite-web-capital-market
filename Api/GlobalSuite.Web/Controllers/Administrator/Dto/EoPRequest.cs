using System;

namespace GlobalSuite.Web.Controllers.Dto
{
    public class EoPRequest
    {
        public EoPRequest()
        {
            RunDate = DateTime.Now;
        }
        public DateTime RunDate { get; set; } 
    }

     
}