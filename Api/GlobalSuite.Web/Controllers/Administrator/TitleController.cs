using System.Threading.Tasks;
using System.Web.Http;
using GlobalSuite.Core;

namespace GlobalSuite.Web.Controllers
{
    public partial class AdministratorController 
    {
        /// <summary>
        /// Get All Titles
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route(GlobalSuiteRoutes.AdministratorRoutes.MaintainRoutes.Titles)]
        [System.Web.Mvc.OutputCache(CacheProfile = Constants.Caching.Cache1Day)]
        public async Task<IHttpActionResult> GetTitles()
        {   
            var countries = await _adminService.GetAllTitles();

            return Ok(countries);
        }
        /// <summary>
        /// Add New Title
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route(GlobalSuiteRoutes.AdministratorRoutes.MaintainRoutes.Titles)]
        public async Task<IHttpActionResult> AddTitle(string name)
        {
            var result = await _adminService.CreateTitle(name);
            if (!result.IsSuccess) return BadRequest(result.ToString());
            return Ok();
        }
        
    }
}
