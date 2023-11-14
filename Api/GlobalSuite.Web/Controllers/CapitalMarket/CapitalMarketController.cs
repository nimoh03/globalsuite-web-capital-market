using AutoMapper;
using GlobalSuite.Core.CapitalMarket;

namespace GlobalSuite.Web.Controllers.CapitalMarket
{
    public partial class CapitalMarketController
    {
        public CapitalMarketController(ITradingService tradingService, IMapper mapper)
        {
            _tradingService = tradingService;
            _mapper = mapper;
        }
    }
}