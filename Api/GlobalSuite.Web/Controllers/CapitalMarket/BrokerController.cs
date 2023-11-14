using System.Threading.Tasks;
using System.Web.Http;
using CapitalMarket.Business;
using GlobalSuite.Web.Controllers.CapitalMarket.Dto;

namespace GlobalSuite.Web.Controllers.CapitalMarket
{
    public partial class CapitalMarketController : BaseController
    {
        [HttpGet, Route(GlobalSuiteRoutes.CapitalMarketRoutes.MaintainRoutes.Brokers)]
        public async Task<IHttpActionResult> Get(string code)
        {
            var broker = await _tradingService.GetBroker(code);
            if (broker==null) return NotFound();
            var response = _mapper.Map<BrokerRequest>(broker);
            return Ok(response);
        } 
          [HttpPost, Route(GlobalSuiteRoutes.CapitalMarketRoutes.MaintainRoutes.Brokers)]
        public async Task<IHttpActionResult> Add([FromBody] BrokerRequest request)
        {
            var broker = _mapper.Map<Broker>(request);
            var result = await _tradingService.AddBroker(broker);
            if (!result.IsSuccess) return BadRequest(result.ToString());
            return Ok();
        } 
        [HttpPut, Route(GlobalSuiteRoutes.CapitalMarketRoutes.MaintainRoutes.Brokers)]
        public async Task<IHttpActionResult> Edit([FromBody] BrokerRequest request)
        {
            var broker = _mapper.Map<Broker>(request);
            var result = await _tradingService.EditBroker(broker);
            if (!result.IsSuccess) return BadRequest(result.ToString());
            return Ok();
        } 
           [HttpDelete, Route(GlobalSuiteRoutes.CapitalMarketRoutes.MaintainRoutes.Brokers)]
        public async Task<IHttpActionResult> Remove(string code)
        {
            var result = await _tradingService.DeleteBroker(code);
            if (!result.IsSuccess) return BadRequest(result.ToString());
            return Ok();
        } 
        
       
    }
}