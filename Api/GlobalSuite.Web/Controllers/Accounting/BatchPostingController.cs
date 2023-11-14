using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using BaseUtility.Business;
using GL.Business;
using GlobalSuite.Core.Accounting;
using GlobalSuite.Core.Accounting.BatchPosting;
using GlobalSuite.Core.Accounting.Models;
using GlobalSuite.Web.Controllers.Accounting.Dto;

namespace GlobalSuite.Web.Controllers.Accounting
{
    public partial class AccountingController
    {
        
        /// <summary>
        /// Get All Batch Posting
        /// </summary>
        [HttpGet, Route(GlobalSuiteRoutes.AccountingRoutes.JournalPostingRoutes.BatchPosting)]
        [ResponseType(typeof(List<BatchPostingResponse>))]
        public async Task<IHttpActionResult> GetBatchPostings([FromUri] BatchPostingFilter filter)
        {
             
                var batchPostings = await _accountingService.GetBatchPostings(filter);
                return Ok(batchPostings);
             
        }  
        
        /// <summary>
        /// Get Batch Posting
        /// </summary> 
        [HttpGet, Route(GlobalSuiteRoutes.AccountingRoutes.JournalPostingRoutes.BatchPosting+"/{batchNo}")]
        [ResponseType(typeof(BatchPostingDetail))]
        public async Task<IHttpActionResult> GetBatchPosting(long batchNo, string status="UnPosted")
        {
            try
            {
                if (status.ToLower() == "all") status = DataGeneral.PostStatus.UnPosted.ToString();
                Enum.TryParse(status, true, out DataGeneral.PostStatus postStatus);
                var batchPosting = await _accountingService.GetBatchPosting(batchNo, postStatus);
                if (batchPosting == null) return NotFound();
                return Ok(batchPosting);
            } 
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                return BadRequest($"{nameof(status)} is not valid");
            }
        }  
        
        
         /// <summary>
        /// Delete Batch Posting
        /// </summary>
        [HttpDelete, Route(GlobalSuiteRoutes.AccountingRoutes.JournalPostingRoutes.BatchPosting+"/{batchNo}")]
        public async Task<IHttpActionResult> RemoveBatchPosting(long batchNo)
        {
                var result = await _accountingService.DeleteBatchPosting(batchNo);
                if (!result.IsSuccess) return BadRequest(result.ToString());
                return Ok();
        } 
        
        
        /// <summary>
        /// Create Batch Posting
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route(GlobalSuiteRoutes.AccountingRoutes.JournalPostingRoutes.BatchPosting)]
        public async Task<IHttpActionResult> CreateBatchPosting ([FromBody] BatchPosting request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (!request.Transactions.Any()) return BadRequest("Posting must have at least one item");
           var spreadSheets= _mapper.Map<List<BatchSpreadSheet>>(request.Transactions);
           var result=  await _accountingService.CreateBatchPosting(request.EffectiveDate,spreadSheets );
           if (!result.IsSuccess) return BadRequest(result.ToString());
            
            return Ok();
        } 
        /// <summary>
        /// Approve/Post Batch Posting
        /// </summary>
        [HttpPost, Route(GlobalSuiteRoutes.AccountingRoutes.JournalPostingRoutes.BatchPosting+"/post/{batchNo}")]
        public async Task<IHttpActionResult> PostBatchPosting(long batchNo)
        {
            var result = await _accountingService.PostBatchPosting(batchNo);
            if (!result.IsSuccess) return BadRequest(result.ToString());
            return Ok();
        } 
        
    }
}