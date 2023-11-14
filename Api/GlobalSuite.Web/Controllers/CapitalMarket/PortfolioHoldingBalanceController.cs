using System.Threading.Tasks;
using System.Web.Http;
using CapitalMarket.Business;
using GlobalSuite.Web.Controllers.CapitalMarket.Dto;

namespace GlobalSuite.Web.Controllers.CapitalMarket
{
    public partial class CapitalMarketController : BaseController
    {
        [HttpPost, Route(GlobalSuiteRoutes.CapitalMarketRoutes.PortfolioRoutes.AddHoldingBalance)]
        public async Task<IHttpActionResult> AddHoldingBalance([FromBody] PortfolioHoldingBalanceRequest request)
        {
            var bal = _mapper.Map<PortOpenBal>(request);
            var result = await _tradingService.PortfolioHoldingBalance(bal);
            if (!result.IsSuccess) return BadRequest(result.ToString());
            return Ok();
        }
        [HttpPost, Route(GlobalSuiteRoutes.CapitalMarketRoutes.PortfolioRoutes.PostHoldingBalance)]
        public async Task<IHttpActionResult> PostHoldBalance(string code)
        {
            var result = await _tradingService.PostPortfolioHoldingBalance(code);
            if (!result.IsSuccess) return BadRequest(result.ToString());
            return Ok();
        }
    }
}