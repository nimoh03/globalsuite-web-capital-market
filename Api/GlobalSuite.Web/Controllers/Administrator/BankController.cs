using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using GlobalSuite.Core;
using GlobalSuite.Core.Admin.Models;

namespace GlobalSuite.Web.Controllers
{
    public partial class AdministratorController 
    {
        /// <summary>
        /// Get All Banks
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route(GlobalSuiteRoutes.AdministratorRoutes.MaintainRoutes.Banks)]
        [System.Web.Mvc.OutputCache(CacheProfile = Constants.Caching.Cache1Day)]
        [ResponseType(typeof(List<Param>))]
        public async Task<IHttpActionResult> GetBank()=> Ok(await _adminService.GetAllBanks());

        /// <summary>
        /// ADD New Bank
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route(GlobalSuiteRoutes.AdministratorRoutes.MaintainRoutes.Banks)]
        public async Task<IHttpActionResult> AddBank(string name)
        {
            var result = await _adminService.CreateBank(name);
            if (!result.IsSuccess) return BadRequest(result.ToString());
            return Ok();
        }

    }
}
