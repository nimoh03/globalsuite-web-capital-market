using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using BaseUtility.Business;
using GL.Business;
using GlobalSuite.Core.Accounting;
using GlobalSuite.Core.Accounting.Models;
using GlobalSuite.Web.Controllers.Accounting.Dto;

namespace GlobalSuite.Web.Controllers.Accounting
{
    public partial class AccountingController:BaseController
    {
        
        /// <summary>
        /// Get All Self Balances
        /// </summary>
        [HttpGet, Route(GlobalSuiteRoutes.AccountingRoutes.JournalPostingRoutes.SelfBalancing)]
        [ResponseType(typeof(List<SelfBalanceResponse>))]
        public async Task<IHttpActionResult> GetSelfBalances([FromUri] StatusFilter filter)
        {
             
                var selfBalances = await _accountingService.GetSelfBalances(filter);
                return Ok(selfBalances);
             
        }  
        
        /// <summary>
        /// Get Self Balance
        /// </summary>
        [HttpGet, Route(GlobalSuiteRoutes.AccountingRoutes.JournalPostingRoutes.SelfBalancing+"/{code}/{status}")]
        [ResponseType(typeof(SelfBalanceResponse))]
        public async Task<IHttpActionResult> GetSelfBalancing(string code, string status)
        {
            try
            {
                if (status.ToLower() == "all") status = DataGeneral.PostStatus.UnPosted.ToString();
                Enum.TryParse(status, true, out DataGeneral.PostStatus postStatus);
                var selfBal = await _accountingService.GetSelfBalance(code, postStatus);
                if (selfBal == null) return NotFound();
                return Ok(selfBal);
            } 
        catch (Exception ex)
        {
            Logger.Error(ex.Message, ex);
            return BadRequest($"{nameof(status)} is not valid");
        }
        }  
        /// <summary>
        /// Delete Self Balance
        [HttpDelete, Route(GlobalSuiteRoutes.AccountingRoutes.JournalPostingRoutes.SelfBalancing+"/{code}")]
        public async Task<IHttpActionResult> Remove(string code)
        {
                var result = await _accountingService.DeleteSelfBalance(code);
                if (!result.IsSuccess) return BadRequest(result.ToString());
                return Ok();
        } 
        /// <summary>
        /// Create Self Balance
        /// </summary>
        [HttpPost, Route(GlobalSuiteRoutes.AccountingRoutes.JournalPostingRoutes.SelfBalancing)]
        public async Task<IHttpActionResult> CreateSelfBalance ([FromBody] SelfBalanceRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var selfBal = _mapper.Map<SelfBal>(request);
            var result = await _accountingService.CreateSelfBalance(selfBal);
            if (!result.IsSuccess) return BadRequest(result.ToString());
            return Ok();
        } 
        
        /// <summary>
        /// Update Self Balance
        /// </summary>
        [HttpPost, Route(GlobalSuiteRoutes.AccountingRoutes.JournalPostingRoutes.SelfBalancing+"/edit")]
        public async Task<IHttpActionResult> EditSelfBalance ([FromBody] SelfBalanceRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var selfBal = _mapper.Map<SelfBal>(request);
            var result = await _accountingService.EditSelfBalance(selfBal);
            if (!result.IsSuccess) return BadRequest(result.ToString());
            return Ok();
        } 
        /// <summary>
        /// Approve/Post Self Balance
        /// </summary>
        [HttpPost, Route(GlobalSuiteRoutes.AccountingRoutes.JournalPostingRoutes.SelfBalancing+"/post/{code}")]
        public async Task<IHttpActionResult> Post(string code)
        {
            var result = await _accountingService.PostSelfBalance(code);
            if (!result.IsSuccess) return BadRequest(result.ToString());
            return Ok();
        } 
        /// <summary>
        /// Reverse Self Balance
        /// </summary>
        [HttpPost, Route(GlobalSuiteRoutes.AccountingRoutes.JournalPostingRoutes.SelfBalancing+"/{code}/reverse")]
        public async Task<IHttpActionResult> ReverseSelfBalance(string code)
        {
            var result = await _accountingService.PostSelfBalance(code);
            if (!result.IsSuccess) return BadRequest(result.ToString());
            return Ok();
        }
    }
}