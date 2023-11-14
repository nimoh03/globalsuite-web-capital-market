using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using AutoMapper;
using GlobalSuite.Web.Controllers.Customers.Dto;

namespace GlobalSuite.Web.Controllers.Customers
{
    public partial class CustomerController
    {
        /// <summary>
        /// Get All KYC Document Types
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route(GlobalSuiteRoutes.CustomerRoutes.CustomerKyc +"/doc-types")]
        public async Task<IHttpActionResult> GetDocTypes()
        {
            var types = await _customerService.GetKycDocTypes();
            var output = _mapper.Map<List<KycDocTypeResponse>>(types);
            return Ok(output);
        }
        /// <summary>
        /// Set KYC Document Type For Customer
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route(GlobalSuiteRoutes.CustomerRoutes.CustomerKyc +"/set-types")]
        public async Task<IHttpActionResult> SetDocTypeForCustomerType([FromBody] KycDocTypeCustomerTypeRequest request)
        {
            if (!ModelState.IsValid) return BadRequest("Request not valid.");
            var result = await _customerService.SetKycDocTypeForCustomerType(request.KyDocTypeId, request.CustomerTypeId, request.IsOptional);
            if (!result.IsSuccess) return BadRequest(result.ToString());
            return Ok(result);
        }
/// <summary>
/// Set Compulsory KYC Document Types
/// </summary>
/// <param name="request"></param>
/// <returns></returns>
        [HttpPost, Route(GlobalSuiteRoutes.CustomerRoutes.CustomerKyc +"/set-types/compulsory")]
        public async Task<IHttpActionResult> SetCompulsoryCustomerKyc(
            [FromBody] List<CompulsoryCustomerKyc> request)
        {
            if (!ModelState.IsValid) return BadRequest("Request not valid.");
            var compulsoryKyc = 
                request.Select(item => (item.FieldId, item.FieldName, item.DataType)).ToList();

            var result=  await _customerService.SetCompulsoryKyc(compulsoryKyc);
          if (!result.IsSuccess) return BadRequest(result.ToString());
          return Ok(result);
        }
    }
}
