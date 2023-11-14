using System.Threading.Tasks;
using System.Web.Http;
using CapitalMarket.Business;
using GlobalSuite.Web.Controllers.CapitalMarket.Dto;

namespace GlobalSuite.Web.Controllers.CapitalMarket
{
    public partial class CapitalMarketController : BaseController
    {

        [HttpGet, Route(GlobalSuiteRoutes.CapitalMarketRoutes.MaintainRoutes.Stocks)]
        public async Task<IHttpActionResult> GetStocks(string code)
        {
            var stock = await _tradingService.GetStock(code);
            if (stock==null) return NotFound();
            var response = _mapper.Map<StockRequest>(stock);
            return Ok(response);
        } 
        [HttpPost, Route(GlobalSuiteRoutes.CapitalMarketRoutes.MaintainRoutes.Stocks)]
        public async Task<IHttpActionResult> AddStocks([FromBody] StockRequest request)
        {
            var stock = _mapper.Map<Stock>(request);
            var result = await _tradingService.AddStock(stock);
            if (!result.IsSuccess) return BadRequest(result.ToString());
            return Ok();
        } 
        [HttpPut, Route(GlobalSuiteRoutes.CapitalMarketRoutes.MaintainRoutes.Stocks)]
        public async Task<IHttpActionResult> EditStock([FromBody] StockRequest request)
        {
            var stock = _mapper.Map<Stock>(request);
            var result = await _tradingService.EditStock(stock);
            if (!result.IsSuccess) return BadRequest(result.ToString());
            return Ok();
        } 
        [HttpDelete, Route(GlobalSuiteRoutes.CapitalMarketRoutes.MaintainRoutes.Stocks)]
        public async Task<IHttpActionResult> RemoveStock(string code)
        {
            var result = await _tradingService.DeleteStock(code);
            if (!result.IsSuccess) return BadRequest(result.ToString());
            return Ok();
        } 
        
       
    }
}