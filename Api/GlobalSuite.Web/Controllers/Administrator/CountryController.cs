using System.Threading.Tasks;
using System.Web.Http;
using GlobalSuite.Core;

namespace GlobalSuite.Web.Controllers
{
    public partial class AdministratorController 
    {

        [HttpGet, Route(GlobalSuiteRoutes.AdministratorRoutes.MaintainRoutes.Countries)]
        [System.Web.Mvc.OutputCache(CacheProfile = Constants.Caching.Cache1Day)]
        public async Task<IHttpActionResult> GetCountries()
        {   
            var countries = await _adminService.GetAllCountries();

            return Ok(countries);
        }

        [HttpGet, Route(GlobalSuiteRoutes.AdministratorRoutes.MaintainRoutes.Countries+"order/name")]
        public async Task<IHttpActionResult> GetCountriesOrderByName()
        {
            var countries = await _adminService.GetAllCountriesOrderByName();

            return Ok(countries);
        }

    }
}
