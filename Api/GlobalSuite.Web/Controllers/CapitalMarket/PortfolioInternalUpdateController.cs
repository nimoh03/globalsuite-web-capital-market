using System.Threading.Tasks;
using System.Web.Http;
using CapitalMarket.Business;
using GlobalSuite.Web.Controllers.CapitalMarket.Dto;

namespace GlobalSuite.Web.Controllers.CapitalMarket
{
     public partial class CapitalMarketController : BaseController
    {
        [HttpPost, Route(GlobalSuiteRoutes.CapitalMarketRoutes.PortfolioRoutes.AddHoldingBalanceInternal)]
        public async Task<IHttpActionResult> AddHoldingBalanceInternal([FromBody] PortfolioHoldingInternalRequest request)
        {
            var oPortfolioInternalUpdate = _mapper.Map<PortfolioInternalUpdate>(request);
            var result = await _tradingService.PortfolioCreateInternalUpdate(oPortfolioInternalUpdate);
            if (!result.IsSuccess) return BadRequest(result.ToString());
            return Ok();
        }
        [HttpPut, Route(GlobalSuiteRoutes.CapitalMarketRoutes.PortfolioRoutes.DeductHoldingBalanceInternal)]
        public async Task<IHttpActionResult> DeductHoldingBalanceInternal([FromBody] PortfolioHoldingInternalRequest request)
        {
            var oPortfolioInternalUpdate = _mapper.Map<PortfolioInternalUpdate>(request);
            var result = await _tradingService.PortfolioUpdateInternalUpdate(oPortfolioInternalUpdate);
            if (!result.IsSuccess) return BadRequest(result.ToString());
            return Ok();
        }
        [HttpPut, Route(GlobalSuiteRoutes.CapitalMarketRoutes.PortfolioRoutes.PostHoldingBalanceInternal)]
        public async Task<IHttpActionResult> Post(string code)
        {
            var result = await _tradingService.PostPortfolioInternalUpdate(code);
            if (!result.IsSuccess) return BadRequest(result.ToString());
            return Ok();
        }
    }
}