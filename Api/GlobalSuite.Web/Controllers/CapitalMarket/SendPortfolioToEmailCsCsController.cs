using System.Threading.Tasks;
using System.Web.Http;
using CapitalMarket.Business;
using GlobalSuite.Web.Controllers.CapitalMarket.Dto;

namespace GlobalSuite.Web.Controllers.CapitalMarket
{
    public partial class CapitalMarketController : BaseController
    {
        [HttpPost, Route(GlobalSuiteRoutes.CapitalMarketRoutes.PortfolioRoutes.SendPortfolioToEmail)]
        public async Task<IHttpActionResult> Post([FromBody] SendPortfolioToEmailCsCsRequest request)
        {
            var email = _mapper.Map<SendPortfolioToEmail>(request);
            var result = await _tradingService.SendPortfolioToEmailCsCs(email);
            if (!result.IsSuccess) return BadRequest(result.ToString());
            return Ok();
        }
    }
}