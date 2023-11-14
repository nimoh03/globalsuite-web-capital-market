using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using BaseUtility.Business;
using GL.Business;
using GlobalSuite.Core.Accounting.Models;
using GlobalSuite.Web.Controllers.Accounting.Dto;
using PaymentResponse = GlobalSuite.Core.Accounting.Models.PaymentResponse;

namespace GlobalSuite.Web.Controllers.Accounting
{
    public partial class AccountingController:BaseController
    {
        /// <summary>
        /// Get all Payments
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>

        [HttpGet, Route(GlobalSuiteRoutes.AccountingRoutes.Payments)]
        [ResponseType(typeof(List<PaymentResponse>))]
        public async Task<IHttpActionResult> GetPayments([FromUri]PaymentFilter filter)
        {
                var payments = await _accountingService.GetPayments(filter);
              //  var response = _mapper.Map<List<PaymentResponse>>(payments);
                return Ok(payments);
        } 
        
        /// <summary>
        /// Get a payment by TransNo
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpGet, Route(GlobalSuiteRoutes.AccountingRoutes.Payments+"/{code}")]
        [ResponseType(typeof(PaymentResponse))]
        public async Task<IHttpActionResult> GetPayment(string code, string status="UnPosted")
        {
            if (status.ToLower() == "all") status = DataGeneral.PostStatus.UnPosted.ToString();
            Enum.TryParse(status, true, out DataGeneral.PostStatus postStatus);
                var deposit = await _accountingService.GetPayment(code, postStatus);
                if (deposit == null) return NotFound();
                return Ok(deposit);
        } 
        
         
        /// <summary>
        /// Create a Payment
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route(GlobalSuiteRoutes.AccountingRoutes.Payments)]
        public async Task<IHttpActionResult> Payment([FromBody] PaymentRequest request)
        {
            var payment = _mapper.Map<Payment>(request);
            var result = await _accountingService.CreatePayment(payment);
            if (!result.IsSuccess) return BadRequest(result.ToString());
            return Ok(result.Data);
        } 
        
        /// <summary>
        /// Update a Payment
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route(GlobalSuiteRoutes.AccountingRoutes.Payments+"/edit")]
        public async Task<IHttpActionResult> EditPayment([FromBody] PaymentUpdateRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var payment = _mapper.Map<Payment>(request);
            var result = await _accountingService.EditPayment(payment);
            if (!result.IsSuccess) return BadRequest(result.ToString());
            return Ok(result.Data);
        } 
       
        /// <summary>
        ///  Post/Approve a payment
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpPost, Route(GlobalSuiteRoutes.AccountingRoutes.Payments+"/{code}")]
        public async Task<IHttpActionResult> PostPayment(string code)
        {
            var result = await _accountingService.PostPayment(code);
            if (!result.IsSuccess) return BadRequest(result.ToString());
            return Ok();
        }
    }
}