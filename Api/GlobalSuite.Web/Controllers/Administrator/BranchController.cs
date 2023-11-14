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
        /// Get All Branches
        /// </summary>
        /// <returns></returns>
        [HttpGet, System.Web.Mvc.OutputCache(CacheProfile =Constants.Caching.Cache1Day)]
        [Route(GlobalSuiteRoutes.AdministratorRoutes.MaintainRoutes.Branches)]
        [ResponseType(typeof(List<BranchResponse>))]
        public async Task<IHttpActionResult> GetBranches()
        {
            var branches = await _adminService.GetBranches();
            return Ok(branches);
        }
     }
}
