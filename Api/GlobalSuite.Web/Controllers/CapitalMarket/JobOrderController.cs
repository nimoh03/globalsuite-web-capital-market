using System.Threading.Tasks;
using System.Web.Http;
using CapitalMarket.Business;
using GlobalSuite.Web.Controllers.CapitalMarket.Dto;

namespace GlobalSuite.Web.Controllers.CapitalMarket
{
    public partial class CapitalMarketController
    {
/// <summary>
///  Create Job order (Mandate)
/// </summary>
/// <param name="request"></param>
/// <returns></returns>
        [HttpPost, Route(GlobalSuiteRoutes.CapitalMarketRoutes.JobbingRoutes.JobOrders)]
        public async Task<IHttpActionResult> Create([FromBody] JobOrderRequest request)
        {
            var jobOrder = _mapper.Map<JobOrder>(request);
            var result = await _tradingService.CreateJobOrder(request.CustNo, jobOrder);
            if (!result.IsSuccess) return BadRequest(result.ToString());
            return Ok();
        }
    }
}