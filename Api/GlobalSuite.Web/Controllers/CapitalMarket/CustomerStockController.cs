using System.Threading.Tasks;
using System.Web.Http;
using GL.Business;
using GlobalSuite.Web.Controllers.CapitalMarket.Dto;

namespace GlobalSuite.Web.Controllers.CapitalMarket
{
    public partial class CapitalMarketController : BaseController
    {
        [HttpPost, Route(GlobalSuiteRoutes.CapitalMarketRoutes.MaintainRoutes.CustomerAccount)]
        public async Task<IHttpActionResult> Post([FromBody] ProductAccountRequest request)
        {
            var customerStock = _mapper.Map<ProductAcct>(request);
            var result = await _tradingService.AddCustomerStock(customerStock);
            if (!result.IsSuccess) return BadRequest(result.ToString());
            return Ok();
        } 
        
        [HttpDelete, Route(GlobalSuiteRoutes.CapitalMarketRoutes.MaintainRoutes.CustomerAccount)]
        public async Task<IHttpActionResult> PostCustomerStock(string code)
        {
            var result = await _tradingService.DeleteCustomerStock(code);
            if (!result.IsSuccess) return BadRequest(result.ToString());
            return Ok();
        }
    }
}