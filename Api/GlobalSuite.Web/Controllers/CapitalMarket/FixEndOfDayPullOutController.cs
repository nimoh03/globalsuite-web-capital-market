using System.Threading.Tasks;
using System.Web.Http;
using GlobalSuite.Web.Controllers.CapitalMarket.Dto;

namespace GlobalSuite.Web.Controllers.CapitalMarket
{
    public partial class CapitalMarketController : BaseController
    {
        [HttpPost, Route(GlobalSuiteRoutes.CapitalMarketRoutes.TradingRoutes.FixEndOfDayPullout)]
        public async Task<IHttpActionResult> Post([FromBody]FixPullOutRequest request)
        {
            var result = await _tradingService.FixPullOut(request.TradeSummary, 
                request.AccountSummary, request.Portfolio);
            if (!result.IsSuccess) return BadRequest(result.ToString());
            return Ok();
        }
    }
}