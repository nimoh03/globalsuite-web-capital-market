using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace GlobalSuite.Web.Controllers.CapitalMarket
{
    public partial class CapitalMarketController : BaseController
    {
        [HttpPost, Route(GlobalSuiteRoutes.CapitalMarketRoutes.TradingRoutes.FixEndOfDay)]
        public async Task<IHttpActionResult> FixEndOfDay(DateTime date)
        {
            var result = await _tradingService.RunEndOfDayForFix(date);
            if (!result.IsSuccess) return BadRequest(result.ToString());
            return Ok();
        }
    }
}