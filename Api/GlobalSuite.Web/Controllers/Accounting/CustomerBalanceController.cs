using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using BaseUtility.Business;
using GL.Business;
using GlobalSuite.Core.Accounting.Models;
using GlobalSuite.Web.Controllers.Accounting.Dto;

namespace GlobalSuite.Web.Controllers.Accounting
{
    public partial class AccountingController
    {
        /// <summary>
        /// Get Customer Balance
        /// </summary>
        [HttpGet, Route(GlobalSuiteRoutes.AccountingRoutes.CustomerBalance+"/{customerId}/{productId}")]
        [ResponseType(typeof(CustomerBalance))]
        public async Task<IHttpActionResult> GetCustomerBalance(string customerId, string productId)
        {
            if (string.IsNullOrEmpty(customerId)) return BadRequest("Customer number is required.");
            if (string.IsNullOrEmpty(productId)) return BadRequest("Product number is required.");
            var balance = await _accountingService.GetCustomerBalance(customerId,productId);
            return Ok(balance);
        }  
    }
}