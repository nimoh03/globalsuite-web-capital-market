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
    public partial class AccountingController:BaseController
    {
        /// <summary>
        /// Get All Opening Balances
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route(GlobalSuiteRoutes.AccountingRoutes.OpeningBalance)]
        [ResponseType(typeof(List<OpeningBalanceResponse>))]
        public async Task<IHttpActionResult> GetOpeningBalances([FromUri]StatusFilter filter)
        {
            var openingBalances = await _accountingService.GetOpeningBalances(filter);
            return Ok(openingBalances);
        }  
        
        /// <summary>
        /// Get Customer Opening balance
        /// </summary>
        [HttpGet, Route(GlobalSuiteRoutes.AccountingRoutes.OpeningBalance+"/{code}")]
        [ResponseType(typeof(OpeningBalanceResponse))]
        public async Task<IHttpActionResult> GetOpeningBalance(string code, string status="UnPosted")
        {
            if (status.ToLower() == "all") status = DataGeneral.PostStatus.UnPosted.ToString();
            Enum.TryParse(status, true, out DataGeneral.PostStatus postStatus);
                var balance = await _accountingService.GetOpeningBalance(code, postStatus);
                if (balance == null) return NotFound();
                
                return Ok(balance);
        } 
        /// <summary>
        /// Create Customer Opening balance
        /// </summary>
        [HttpPost, Route(GlobalSuiteRoutes.AccountingRoutes.OpeningBalance)]
        public async Task<IHttpActionResult> OpeningBalance([FromBody] OpeningBalanceRequest request)
        {
            var oBal = _mapper.Map<CustOBal>(request);
            var result = await _accountingService.OpeningBalance(oBal);
            if (!result.IsSuccess) return BadRequest(result.ToString());
            return Ok();
        } 
        /// <summary>
        /// Update Customer Opening balance
        /// </summary>
        [HttpPost, Route(GlobalSuiteRoutes.AccountingRoutes.OpeningBalance+"/edit")]
        public async Task<IHttpActionResult> EditOpeningBalance([FromBody] OpeningBalanceRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (string.IsNullOrEmpty(request.Code)) return BadRequest("Code is required.");
            var oBal = _mapper.Map<CustOBal>(request);
            var result = await _accountingService.EditOpeningBalance(oBal);
            if (!result.IsSuccess) return BadRequest(result.ToString());
            return Ok();
        } 
        /// <summary>
        /// Post/Approve Customer Opening balance
        /// </summary>
        [HttpPost, Route(GlobalSuiteRoutes.AccountingRoutes.OpeningBalance+"/post/{code}")]
        public async Task<IHttpActionResult> PostBalance([FromUri]string code)
        {
            var result = await _accountingService.PostOpeningBalance(code);
            if (!result.IsSuccess) return BadRequest(result.ToString());
            return Ok();
        }
        /// <summary>
        /// Post/Approve Customer Opening balance
        /// </summary>
        [HttpPost, Route(GlobalSuiteRoutes.AccountingRoutes.OpeningBalance+"/{code}/reverse")]
        public async Task<IHttpActionResult> ReverseBalance([FromUri]string code)
        {
            var result = await _accountingService.PostOpeningBalance(code);
            if (!result.IsSuccess) return BadRequest(result.ToString());
            return Ok();
        }
    }
}