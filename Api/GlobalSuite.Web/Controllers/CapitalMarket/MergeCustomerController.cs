using System.Threading.Tasks;
using System.Web.Http;
using CapitalMarket.Business;
using GlobalSuite.Web.Controllers.CapitalMarket.Dto;

namespace GlobalSuite.Web.Controllers.CapitalMarket
{
    public partial class CapitalMarketController
    {

        [HttpPost, Route(GlobalSuiteRoutes.CapitalMarketRoutes.MaintainRoutes.MergeCustomer)]
        public async Task<IHttpActionResult> Post([FromBody] MergeCustomerRequest request)
        {
            var customer = _mapper.Map<MergeCustomer>(request);
            var result = await _tradingService.MergeCustomer(customer, request.CustomerAccounts);
            if (!result.IsSuccess) return BadRequest(result.ToString());
            return Ok();
        } 
        
       
    }
}