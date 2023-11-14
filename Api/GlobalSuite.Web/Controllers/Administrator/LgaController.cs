using System.Threading.Tasks;
using System.Web.Http;
using BaseUtility.Business;
using GlobalSuite.Core;
using GlobalSuite.Web.Controllers.Dto;

namespace GlobalSuite.Web.Controllers
{
    public partial class AdministratorController 
    {
/// <summary>
/// Get All LGAs
/// </summary>
/// <returns></returns>
        [HttpGet, Route(GlobalSuiteRoutes.AdministratorRoutes.MaintainRoutes.Lgas)]
        [System.Web.Mvc.OutputCache(CacheProfile = Constants.Caching.Cache1Day)]
        public async Task<IHttpActionResult> Get()
        {
            var lgas = await _adminService.GetAllLgas();

            return Ok(lgas);
        }
        /// <summary>
        /// Create A New LGA
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route(GlobalSuiteRoutes.AdministratorRoutes.MaintainRoutes.Lgas)]
        public async Task<IHttpActionResult> Create([FromBody] LgaRequest request)
        {
            if(!ModelState.IsValid) return BadRequest();
            var result = await _adminService.CreateLga(request.Name, request.StateId);
            if (result == DataGeneral.SaveStatus.NameExistAdd)
                return BadRequest($"{request.Name} exist.");
            return Ok();
        }
        /// <summary>
        /// Get All LGA By State
        /// </summary>
        /// <param name="stateId"></param>
        /// <returns></returns>
        [HttpGet, Route(GlobalSuiteRoutes.AdministratorRoutes.MaintainRoutes.Lgas+"/{stateId}")]
        [System.Web.Mvc.OutputCache(CacheProfile = Constants.Caching.Cache1Day)]
        public async Task<IHttpActionResult> GetState(int stateId)
        {
            var lgas = await _adminService.GetLgaByStateId(stateId);

            return Ok(lgas);
        }



    }
}
