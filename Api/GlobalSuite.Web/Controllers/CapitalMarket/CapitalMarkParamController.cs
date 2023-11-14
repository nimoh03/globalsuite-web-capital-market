using System.Threading.Tasks;
using System.Web.Http;
using CapitalMarket.Business;
using GlobalSuite.Web.Controllers.CapitalMarket.Dto;

namespace GlobalSuite.Web.Controllers.CapitalMarket
{
    public partial class CapitalMarketController:BaseController
    {
        [HttpPost, Route(GlobalSuiteRoutes.CapitalMarketRoutes.MaintainRoutes.CapitalMarketParams)]
        public async Task<IHttpActionResult> Add([FromBody]CapitalMarketRequest request)
        {
            var stkParam = _mapper.Map<StkParam>(request.StkParam);
            var penTable = _mapper.Map<PGenTable>(request.PGenTable);
            var result = await _tradingService.AddCapitalMarketParam(request.MemberCode, stkParam, penTable);
            if (!result.IsSuccess) return BadRequest(result.ToString());
            return Ok();
        }
    }
}