using System.Threading.Tasks;
using System.Web.Http;
using GlobalSuite.Core;

namespace GlobalSuite.Web.Controllers
{
    public partial class AdministratorController 
    {
        /// <summary>
        /// Get All GeoZones
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route(GlobalSuiteRoutes.AdministratorRoutes.MaintainRoutes.GeoPoliticalZones)]
        [System.Web.Mvc.OutputCache(CacheProfile = Constants.Caching.Cache1Day)]
        public async Task<IHttpActionResult> GetGeoZones()=> Ok(await _adminService.GetAllGeoZones());

        /// <summary>
        /// ADD New GeoZones
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route(GlobalSuiteRoutes.AdministratorRoutes.MaintainRoutes.GeoPoliticalZones)]
        public async Task<IHttpActionResult> AddGeoZones(string name)
        {
            var result = await _adminService.CreateGeoZones(name);
            if (!result.IsSuccess) return BadRequest(result.ToString());
            return Ok();
        }

    }
}
