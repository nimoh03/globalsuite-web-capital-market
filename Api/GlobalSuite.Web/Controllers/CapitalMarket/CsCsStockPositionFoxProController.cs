using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace GlobalSuite.Web.Controllers.CapitalMarket
{
     

    public partial class CapitalMarketController : BaseController
    {
        [HttpPost, Route(GlobalSuiteRoutes.CapitalMarketRoutes.PortfolioRoutes.CsCsStockPosition)]
        public async Task<IHttpActionResult> PostCsCsStockPosition(DateTime date)
        {
            var result = await _tradingService.PostCsCsStockPositionFoxPro(date);
            if (!result.IsSuccess) return BadRequest(result.ToString());
            return Ok();
        }
    }
}