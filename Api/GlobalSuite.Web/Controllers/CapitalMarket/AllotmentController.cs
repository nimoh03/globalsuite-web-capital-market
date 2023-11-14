using System.Threading.Tasks;
using System.Web.Http;
using AutoMapper;
using CapitalMarket.Business;
using GlobalSuite.Core.CapitalMarket;
using GlobalSuite.Web.Controllers.CapitalMarket.Dto;

namespace GlobalSuite.Web.Controllers.CapitalMarket
{
    public partial class CapitalMarketController:BaseController
    {
        private readonly ITradingService _tradingService;
        private readonly IMapper _mapper;

        
        [HttpPost, Route(GlobalSuiteRoutes.CapitalMarketRoutes.TradingRoutes.Buy)]
        public async Task<IHttpActionResult> Buy([FromBody] AllotmentRequest request)
        {
            var allotment = _mapper.Map<Allotment>(request);
            var result = await _tradingService.AllotmentBuy(allotment);
            if (!result.IsSuccess) return BadRequest(result.ToString());
            return Ok();
        }
        [HttpPost,Route(GlobalSuiteRoutes.CapitalMarketRoutes.TradingRoutes.Sell)]
        public async Task<IHttpActionResult> Sell([FromBody] AllotmentRequest request)
        {
            var allotment = _mapper.Map<Allotment>(request);
            var result = await _tradingService.AllotmentSell(allotment);
            if (!result.IsSuccess) return BadRequest(result.ToString());
            return Ok();
        }
        [HttpPost, Route(GlobalSuiteRoutes.CapitalMarketRoutes.TradingRoutes.CrossDeals)]
        public async Task<IHttpActionResult> CrossDeal([FromBody] AllotmentCrossDealRequest request)
        {
            var sell = _mapper.Map<Allotment>(request.Sale);
            var buy = _mapper.Map<Allotment>(request.Buy);
            var result = await _tradingService.AllotmentCrossDeal(buy,sell);
            if (!result.IsSuccess) return BadRequest(result.ToString());
            return Ok();
        }
    }
}