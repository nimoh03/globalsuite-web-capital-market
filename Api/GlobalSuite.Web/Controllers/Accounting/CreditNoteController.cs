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
        /// Get All Credit Notes
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route(GlobalSuiteRoutes.AccountingRoutes.CreditNote)]
        [ResponseType(typeof(List<CreditNoteResponse>))]
        public async Task<IHttpActionResult> GetCreditNotes([FromUri]CreditNoteFilter filter)
        {
                var creditNotes = await _accountingService.GetCreditNotes(filter);
                return Ok(creditNotes);
        }  
        /// <summary>
        /// Get Credit Note
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpGet, Route(GlobalSuiteRoutes.AccountingRoutes.CreditNote+"/{code}")]
        [ResponseType(typeof(CreditNoteResponse))]
        public async Task<IHttpActionResult> GetCreditNote(string code, string status="UnPosted")
        {
            try
            {
                if (status.ToLower() == "all") status = DataGeneral.PostStatus.UnPosted.ToString();
                Enum.TryParse(status, true, out DataGeneral.PostStatus postStatus);
                var creditNote = await _accountingService.GetCreditNote(code, postStatus);
                if (creditNote == null) return NotFound();
                return Ok(creditNote);
                
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                return BadRequest($"{nameof(status)} is not valid");
            }
        } 
        /// <summary>
        /// Create Credit Note
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route(GlobalSuiteRoutes.AccountingRoutes.CreditNote)]
        public async Task<IHttpActionResult> CreditNote([FromBody] CreditNoteRequest request)
        {
            var creditNote = _mapper.Map<CNote>(request);
            var result = await _accountingService.CreditNote(creditNote);
            if (!result.IsSuccess) return BadRequest(result.ToString());
            return Ok();
        } 
        /// <summary>
        /// Update Credit Note
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route(GlobalSuiteRoutes.AccountingRoutes.CreditNote+"/edit")]
        public async Task<IHttpActionResult> EditCreditNote([FromBody] CreditNoteRequest request)
        {
            var creditNote = _mapper.Map<CNote>(request);
            var result = await _accountingService.EditCreditNote(creditNote);
            if (!result.IsSuccess) return BadRequest(result.ToString());
            return Ok();
        } 
        /// <summary>
        /// Post/Approve Credit Notes
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route(GlobalSuiteRoutes.AccountingRoutes.CreditNote+"/post/{code}")]
        public async Task<IHttpActionResult> PostCredit(string code)
        {
            var result = await _accountingService.PostCreditNote(code);
            if (!result.IsSuccess) return BadRequest(result.ToString());
            return Ok();
        }
        /// <summary>
        /// Reverse Credit Note
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route(GlobalSuiteRoutes.AccountingRoutes.CreditNote+"/{code}/reverse")]
        public async Task<IHttpActionResult> ReverseCredit(string code)
        {
            var result = await _accountingService.ReverseCreditNote(code);
            if (!result.IsSuccess) return BadRequest(result.ToString());
            return Ok();
        }
    }
}