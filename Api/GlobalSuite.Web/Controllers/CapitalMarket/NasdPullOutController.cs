using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace GlobalSuite.Web.Controllers.CapitalMarket
{
    public partial class CapitalMarketController : BaseController
    {
        [HttpPost, Route(GlobalSuiteRoutes.CapitalMarketRoutes.TradingRoutes.NasdPostingPullout)]
        public async Task<IHttpActionResult> PostPullout(DateTime tradeDate)
        {
            var result = await _tradingService.NasdPullOut(tradeDate);
            if (!result.IsSuccess) return BadRequest(result.ToString());
            return Ok();
        }
    }
}