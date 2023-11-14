using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace GlobalSuite.Web.Controllers.CapitalMarket
{
    public partial class CapitalMarketController : BaseController
    {
        [HttpPost, Route(GlobalSuiteRoutes.CapitalMarketRoutes.TradingRoutes.DailyPostingPullout)]
        public async Task<IHttpActionResult> Post(DateTime tradeDate)
        {
            var result = await _tradingService.DailyTradePullOut(tradeDate);
            if (!result.IsSuccess) return BadRequest(result.ToString());
            return Ok();
        }
    }
}