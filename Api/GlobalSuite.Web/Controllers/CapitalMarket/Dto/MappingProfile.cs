using AutoMapper;
using CapitalMarket.Business;
using GL.Business;

namespace GlobalSuite.Web.Controllers.CapitalMarket.Dto
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<AllotmentRequest, Allotment>();
            CreateMap<AllotmentCrossDealRequest, Allotment>();
            CreateMap<StkParamRequest, StkParam>();
            CreateMap<PGenTableRequest, PGenTable>();
            CreateMap<TradeValuationRequest, Valuation>();
            CreateMap<PortfolioHoldingBalanceRequest, PortOpenBal>();
            CreateMap<PortfolioHoldingInternalRequest, PortfolioInternalUpdate>();
            CreateMap<SendPortfolioToEmailCsCsRequest, SendPortfolioToEmail>();
            CreateMap<ProductAccountRequest, ProductAcct>();
            CreateMap<StockRequest, Stock>().ReverseMap();
            CreateMap<BrokerRequest, Broker>().ReverseMap();
            CreateMap<JobOrderRequest, JobOrder>();
        }
    }
}