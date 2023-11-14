using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using GL.Business;
using GlobalSuite.Core.Accounting.Models;
using GlobalSuite.Core.Helpers;
using GlobalSuite.Web.Controllers.Accounting.Dto;
using GlobalSuite.Web.Responses;

namespace GlobalSuite.Web.Controllers.Accounting
{
    public partial class AccountingController
    {
        /// <summary>
     /// Get All Deposits
     /// </summary>
     /// <param name="filter"></param>
     /// <returns></returns>
        [HttpGet, Route(GlobalSuiteRoutes.AccountingRoutes.Deposits)]
        [ResponseType(typeof(List<DepositResponse>))]
        public async Task<IHttpActionResult> GetDeposits([FromUri]DepositFilter filter)
        {
                var deposits = await _accountingService.GetDeposits(filter);
                var response = _mapper.Map<List<DepositResponse>>(deposits);
                return Ok(response);
        } 
        /// <summary>
        /// Get Deposit by TransNo
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpGet, Route(GlobalSuiteRoutes.AccountingRoutes.Deposits+"/{code}")]
        [ResponseType(typeof(DepositDetailResponse))]
        public async Task<IHttpActionResult> GetDeposit(string code)
        {
                var deposit = await _accountingService.GetDeposit(code);
                if (deposit == null) return NotFound();
                return Ok(deposit);
        } 
        /// <summary>
        /// Create/Add a new deposit
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route(GlobalSuiteRoutes.AccountingRoutes.Deposits)]
        [ResponseType(typeof(ApiResponse))]
        [ResponseType(typeof(ApiError))]
        public async Task<IHttpActionResult> Deposit([FromBody] DepositRequest request)
        {
            var deposit = _mapper.Map<Deposit>(request);
            var result = await _accountingService.Deposit(deposit);
            if (!result.IsSuccess) return BadRequest(result.ToString());
            return Ok(result.Data);
        } 
        /// <summary>
        /// Edit/Update a deposit
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route(GlobalSuiteRoutes.AccountingRoutes.Deposits+"/edit")]
        [ResponseType(typeof(ApiResponse))]
        public async Task<IHttpActionResult> Deposit([FromBody] DepositUpdateRequest request)
        {
            var deposit = _mapper.Map<Deposit>(request);
            var result = await _accountingService.EditDeposit(deposit);
            if (!result.IsSuccess) return BadRequest(result.ToString());
            return Ok(result.Data);
        } 
        /// <summary>
        /// Post/Approve a deposit
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpPost, Route(GlobalSuiteRoutes.AccountingRoutes.Deposits+"/{code}")]
        [ResponseType(typeof(ApiResponse))]
        [ResponseType(typeof(ApiError))]
        public async Task<IHttpActionResult> PostDeposit(string code)
        {
            var result = await _accountingService.PostDeposit(code);
            if (!result.IsSuccess) return BadRequest(result.ToString());
            return Ok();
        } 
        /// <summary>
        /// Reverse a deposit
        /// </summary>
        /// <paramref name="code"></paramref>
        [HttpPost, Route(GlobalSuiteRoutes.AccountingRoutes.Deposits+"/{code}/reverse")]
        [ResponseType(typeof(ApiResponse))] 
        public async Task<IHttpActionResult> Reverse(string code)
        {
            var result = await _accountingService.ReverseDeposit(code);
            if (!result.IsSuccess) return BadRequest(result.ToString());
            return Ok();
        } 
        /// <summary>
        /// Delete a deposit
        /// </summary>
        /// <paramref name="code"></paramref>
        [HttpDelete, Route(GlobalSuiteRoutes.AccountingRoutes.Deposits+"/{code}")]
        [ResponseType(typeof(ApiResponse))] 
        public async Task<IHttpActionResult> Delete(string code)
        {
            var result = await _accountingService.DeleteDeposit(code);
            if (!result.IsSuccess) return BadRequest(result.ToString());
            return Ok();
        }
    }
}