using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using GL.Business;
using GlobalSuite.Core.Accounting;
using GlobalSuite.Web.Controllers.Accounting.Dto;

namespace GlobalSuite.Web.Controllers.Accounting
{
    public partial class AccountingController:BaseController
    {
        /// <summary>
        /// Get All GL Param Setting
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route(GlobalSuiteRoutes.AccountingRoutes.MaintainRoutes.GlParamSetting)]
        [ResponseType(typeof(GlParamResponse))]
        public async Task<IHttpActionResult> Get()
        {
                var glParam = await _accountingService.GetGlParam();
                if (glParam == null) return NotFound();
                return Ok(glParam);
        }  
        /// <summary>
        /// Update GL Param Setting
        /// </summary>
        [HttpPost, Route(GlobalSuiteRoutes.AccountingRoutes.MaintainRoutes.GlParamSetting)]
        public async Task<IHttpActionResult> GlParam([FromBody] GlParamRequest request)
        {
            var oGlParam = _mapper.Map<GLParam>(request);
            var result = await _accountingService.EditGlParam(oGlParam);
            if (!result.IsSuccess) return BadRequest(result.ToString());
            return Ok();
        } 
    }
}