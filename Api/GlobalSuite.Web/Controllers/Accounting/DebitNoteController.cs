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
    {/// <summary>
        /// Get All Debit Notes
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route(GlobalSuiteRoutes.AccountingRoutes.DebitNote)]
        [ResponseType(typeof(List<DebitNoteResponse>))]
        public async Task<IHttpActionResult> GetDebitNotes([FromUri]DebitNoteFilter filter)
        {
            var debitNotes = await _accountingService.GetDebitNotes(filter);
            return Ok(debitNotes);
        }  
        /// <summary>
        /// Get Debit Note
        /// </summary>
        [HttpGet, Route(GlobalSuiteRoutes.AccountingRoutes.DebitNote+"/{code}")]
        [ResponseType(typeof(DebitNoteResponse))]
        public async Task<IHttpActionResult> GetDebitNote(string code, string status="UnPosted")
        {
           
            try
            {
                if (status.ToLower() == "all") status = DataGeneral.PostStatus.UnPosted.ToString();
                Enum.TryParse(status, true, out DataGeneral.PostStatus postStatus);
                var debitNote = await _accountingService.GetDebitNote(code, postStatus);
                if (debitNote == null) return NotFound();
                return Ok(debitNote);
                
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                return BadRequest($"{nameof(status)} is not valid");
            }
        } 
        
        /// <summary>
        /// Create Debit Note
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route(GlobalSuiteRoutes.AccountingRoutes.DebitNote)]
        public async Task<IHttpActionResult> CreateDebitNote([FromBody] DebitNoteRequest request)
        {
            var debitNote = _mapper.Map<DNote>(request);
            var result = await _accountingService.DebitNote(debitNote);
            if (!result.IsSuccess) return BadRequest(result.ToString());
            return Ok();
        } 
        /// <summary>
        /// Update Debit Note
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route(GlobalSuiteRoutes.AccountingRoutes.DebitNote+"/edit")]
        public async Task<IHttpActionResult> EditDebitNote([FromBody] DebitNoteRequest request)
        {
            var debitNote = _mapper.Map<DNote>(request);
            var result = await _accountingService.EditDebitNote(debitNote);
            if (!result.IsSuccess) return BadRequest(result.ToString());
            return Ok();
        } 
        /// <summary>
        /// Post/Approve Debit Note
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route(GlobalSuiteRoutes.AccountingRoutes.DebitNote+"/post/{code}")]
        public async Task<IHttpActionResult> PostDebit(string code)
        {
            var result = await _accountingService.PostDebitNote(code);
            if (!result.IsSuccess) return BadRequest(result.ToString());
            return Ok();
        } 
        /// <summary>
        /// Reverse Debit Note
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route(GlobalSuiteRoutes.AccountingRoutes.DebitNote+"/{code}/reverse")]
        public async Task<IHttpActionResult> ReverseDebit(string code)
        {
            var result = await _accountingService.ReverseDebitNote(code);
            if (!result.IsSuccess) return BadRequest(result.ToString());
            return Ok();
        }
    }
}