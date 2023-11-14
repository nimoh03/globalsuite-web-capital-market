using System.Threading.Tasks;
using System.Web.Http;
using CapitalMarket.Business;
using GlobalSuite.Web.Controllers.CapitalMarket.Dto;

namespace GlobalSuite.Web.Controllers.CapitalMarket
{
    public partial class CapitalMarketController : BaseController
    {
        [HttpPost, Route(GlobalSuiteRoutes.CapitalMarketRoutes.TradingRoutes.TradeValuation)]
        public async Task<IHttpActionResult> PostTradeValuation([FromBody] TradeValuationRequest request)
        {
            var valuation = _mapper.Map<Valuation>(request);
            var result = await _tradingService.TradeValuation(valuation);
            if (!result.IsSuccess) return BadRequest(result.ToString());
            return Ok();
        }
    }
}