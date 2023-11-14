using System.Threading.Tasks;
using System.Web.Http;
using GlobalSuite.Core;

namespace GlobalSuite.Web.Controllers
{
    public partial class AdministratorController 
    {
        /// <summary>
        /// Get All Religions
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route(GlobalSuiteRoutes.AdministratorRoutes.MaintainRoutes.Religions)]
        [System.Web.Mvc.OutputCache(CacheProfile = Constants.Caching.Cache1Day)]
        public async Task<IHttpActionResult> GetReligion()=> Ok(await _adminService.GetAllReligions());

        /// <summary>
        /// ADD New Religion
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route(GlobalSuiteRoutes.AdministratorRoutes.MaintainRoutes.Religions)]
        public async Task<IHttpActionResult> AddReligion(string name)
        {
            var result = await _adminService.CreateReligion(name);
            if (!result.IsSuccess) return BadRequest(result.ToString());
            return Ok();
        }

    }
}
