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
        /// Get All Customer Transfers
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route(GlobalSuiteRoutes.AccountingRoutes.Transfer)]
        [ResponseType(typeof(List<TransferResponse>))]
        public async Task<IHttpActionResult> GetTransfers([FromUri] TransferFilter filter)
        {
                var transfers = await _accountingService.GetTransfers(filter);
                return Ok(transfers);
        } 
        /// <summary>
        /// Get Customer Transfer
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route(GlobalSuiteRoutes.AccountingRoutes.Transfer+"/{code}")]
        [ResponseType(typeof(TransferResponse))]
        public async Task<IHttpActionResult> GetTransfer(string code, string status="UnPosted")
        {
            try
            {
                if (status.ToLower() == "all") status = DataGeneral.PostStatus.UnPosted.ToString();
                Enum.TryParse(status, true, out DataGeneral.PostStatus postStatus);
                var transfer = await _accountingService.GetTransfer(code, postStatus);
                if (transfer == null) return NotFound();
                return Ok(transfer);
                
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                return BadRequest($"{nameof(status)} is not valid.");
            }
               
        } 
        /// <summary>
        /// Create Customer Transfer
        /// </summary>
        [HttpPost, Route(GlobalSuiteRoutes.AccountingRoutes.Transfer)]
        public async Task<IHttpActionResult> CustomerTransfer([FromBody] CustomerTransferRequest request)
        {
            var customerTransfer = _mapper.Map<CustomerTransfer>(request);
            var result = await _accountingService.CreateTransfer(customerTransfer);
            if (!result.IsSuccess) return BadRequest(result.ToString());
            return Ok();
        } 
        /// <summary>
        /// Approve/Post Customer Transfer
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpPost, Route(GlobalSuiteRoutes.AccountingRoutes.Transfer+"/post/{code}")]
        public async Task<IHttpActionResult> PostTransfer(string code)
        {
            var result = await _accountingService.PostTransfer(code);
            if (!result.IsSuccess) return BadRequest(result.ToString());
            return Ok();
        }
        
        /// <summary>
        /// Reverse Customer Transfer
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpPost, Route(GlobalSuiteRoutes.AccountingRoutes.Transfer+"/{code}/reverse")]
        public async Task<IHttpActionResult> ReverseTransfer(string code)
        {
            var result = await _accountingService.ReverseTransfer(code);
            if (!result.IsSuccess) return BadRequest(result.ToString());
            return Ok();
        }
    }
}