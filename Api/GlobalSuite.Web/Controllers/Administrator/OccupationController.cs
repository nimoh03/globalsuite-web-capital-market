using System.Threading.Tasks;
using System.Web.Http;
using GlobalSuite.Core;

namespace GlobalSuite.Web.Controllers
{
    public partial class AdministratorController 
    {
        /// <summary>
        /// Get All Occupations
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route(GlobalSuiteRoutes.AdministratorRoutes.MaintainRoutes.Occupations)]
        [System.Web.Mvc.OutputCache(CacheProfile = Constants.Caching.Cache1Day)]
        public async Task<IHttpActionResult> GetOccupation()=> Ok(await _adminService.GetAllOccupations());

        /// <summary>
        /// ADD New Occupation
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route(GlobalSuiteRoutes.AdministratorRoutes.MaintainRoutes.Occupations)]
        public async Task<IHttpActionResult> AddOccupation(string name)
        {
            var result = await _adminService.CreateOccupation(name);
            if (!result.IsSuccess) return BadRequest(result.ToString());
            return Ok();
        }

    }
}
