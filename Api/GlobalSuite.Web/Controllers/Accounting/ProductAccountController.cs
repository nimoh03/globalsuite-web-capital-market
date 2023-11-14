using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using GL.Business;
using GlobalSuite.Core.Accounting.Models;
using GlobalSuite.Web.Controllers.Accounting.Dto;

namespace GlobalSuite.Web.Controllers.Accounting
{
    public partial class AccountingController
    {
        /// <summary>
        /// Get All Subsidiary By Product and Branch codes
        /// </summary>
        /// <paramref name="customerName"/>
        /// <paramref name="productCode"/>
        /// <paramref name="branchCode"/>
        [HttpGet, Route(GlobalSuiteRoutes.AccountingRoutes.ProductAccounts)]
        [ResponseType(typeof(List<ProductAccountResponse>))]
        public async Task<IHttpActionResult> GetProductAccounts(string customerName, string productCode, string branchCode)
        {
            var products = await _accountingService.GetAllSubsidiaryAccounts(productCode, branchCode, customerName);
                return Ok(products);
        } 
    }
}