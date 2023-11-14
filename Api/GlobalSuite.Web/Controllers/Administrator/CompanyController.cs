using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using BaseUtility.Business;
using GL.Business;
using GlobalSuite.Core.Admin;
using GlobalSuite.Core.Exceptions;
using GlobalSuite.Core.Helpers;
using GlobalSuite.Web.Controllers.Dto;

namespace GlobalSuite.Web.Controllers
{
    public partial class AdministratorController
    {
/// <summary>
/// Get All Countries
/// </summary>
/// <returns></returns>
        [HttpGet, Route(GlobalSuiteRoutes.AdministratorRoutes.CompanyRoutes.Companies)]
[ResponseType(typeof(List<CompanyResponse>))]
public async Task<IHttpActionResult> GetAll()
        {
            var comanies = await _adminService.GetAllCompanies();
            return Ok(comanies);
        }
/// <summary>
/// Create a New Company
/// </summary>
/// <param name="request"></param>
/// <returns></returns>
        [HttpPost, Route(GlobalSuiteRoutes.AdministratorRoutes.CompanyRoutes.Companies)]
        public async Task<IHttpActionResult> Create([FromBody] CompanyRequest request)
        {
            if(!ModelState.IsValid) return BadRequest(ModelState);
            var result = ValidateRequest(request);
            if(result.Count > 0) return BadRequest(result.Select(x=>x.Value).ToString());
            try
            {
                var company=_mapper.Map<Company>(request);
              var res=  await  _adminService.SaveCompany(company);
                if (res == DataGeneral.SaveStatus.NotExist)
                    return BadRequest("Cannot Edit, Company Does Not Exist");
                if (res == DataGeneral.SaveStatus.NameExistAdd)
                    return BadRequest("Error Saving Company,Company Name Already Exist.");
                if (res == DataGeneral.SaveStatus.Saved)
                    return Ok();

            }
            catch (AppException ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok();
        }
/// <summary>
/// Run End Of Month
/// </summary>
/// <param name="request"></param>
/// <returns></returns>
        [HttpPost, Route(GlobalSuiteRoutes.AdministratorRoutes.CompanyRoutes.RunEndOfMonth)]
        public async Task<IHttpActionResult> RunEoM([FromBody] EoPRequest request)
        {
            var result = await _adminService.RunEoM(request.RunDate);
            if (!result.IsSuccess) return BadRequest(result.ToString());
            return Ok();
        }
        /// <summary>
        /// Run Start of Month
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route(GlobalSuiteRoutes.AdministratorRoutes.CompanyRoutes.RunStartOfMonth)]
        public async Task<IHttpActionResult> RunSoM([FromBody] EoPRequest request)
        {
            var result = await _adminService.RunSoM(request.RunDate);
            if (!result.IsSuccess) return BadRequest(result.ToString());
            return Ok();
        }
/// <summary>
/// Run End Of Year
/// </summary>
/// <returns></returns>
        [HttpPost, Route(GlobalSuiteRoutes.AdministratorRoutes.CompanyRoutes.RunEndOfYear)]
        public async Task<IHttpActionResult> RunEoY()
        {
            var result = await _adminService.RunEoY();
            if (!result.IsSuccess) return BadRequest(result.ToString());
            return Ok();
        }
        /// <summary>
        /// Run Start Of Year
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route(GlobalSuiteRoutes.AdministratorRoutes.CompanyRoutes.RunStartOfYear)]
        public async Task<IHttpActionResult> RunSoY()
        {
            var result = await _adminService.RunSoY();
            if (!result.IsSuccess) return BadRequest(result.ToString());
            return Ok();
        }
/// <summary>
/// Run Open Close Period
/// </summary>
/// <param name="request"></param>
/// <returns></returns>
        [HttpPost, Route(GlobalSuiteRoutes.AdministratorRoutes.CompanyRoutes.OpenClosedPeriod)]
        public async Task<IHttpActionResult> OpenClosedPeriod([FromBody] ClosedPeriodRequest request)
        {
            var result = await _adminService.OpenClosedPeriod(request.MonthDate, request.YearStartDate, request.YearEndDate);
            if (!result.IsSuccess) return BadRequest(result.ToString());
            return Ok();
        }
/// <summary>
/// Run Close Closed Period
/// </summary>
/// <param name="request"></param>
/// <returns></returns>
        [HttpPost, Route(GlobalSuiteRoutes.AdministratorRoutes.CompanyRoutes.CloseClosedPeriod)]
        public async Task<IHttpActionResult> CloseClosedPeriod([FromBody] ClosedPeriodRequest request)
        {
            var result = await _adminService.CloseClosedPeriod(request.MonthDate, request.YearStartDate, request.YearEndDate);
            if (!result.IsSuccess) return BadRequest(result.ToString());
            return Ok();
        }
/// <summary>
/// Run Open Closed Period - Within Current Year
/// </summary>
/// <param name="request"></param>
/// <returns></returns>
        [HttpPost, Route(GlobalSuiteRoutes.AdministratorRoutes.CompanyRoutes.OpenClosedPeriodYear)]
        public async Task<IHttpActionResult> OpenClosedPeriodCurrentYear([FromBody] ClosedPeriodRequest request)
        {
            var result = await _adminService.OpenClosedPeriodCurrentYear(request.MonthDate, request.YearStartDate, request.YearEndDate);
            if (!result.IsSuccess) return BadRequest(result.ToString());
            return Ok();
        }
       /// <summary>
       /// Run Close Closed Period - Within the Current Year
       /// </summary>
       /// <param name="request"></param>
       /// <returns></returns>
        [HttpPost, Route(GlobalSuiteRoutes.AdministratorRoutes.CompanyRoutes.CloseClosedPeriodYear)]
        public async Task<IHttpActionResult> CloseClosedPeriodCurrentYear([FromBody] ClosedPeriodRequest request)
        {
            var result = await _adminService.CloseClosedPeriodCurrentYear(request.MonthDate, request.YearStartDate, request.YearEndDate);
            if (!result.IsSuccess) return BadRequest(result.ToString());
            return Ok();
        }
        private Dictionary<string, string> ValidateRequest(CompanyRequest request)
        {
            var res = new Dictionary<string, string>();
            if (request.StartYear == null || request.EndYear == null)
            {   res["YearNotSet"] = "Cannot Save Company,End Of Year Or Start Of Year Date Cannot Be Empty";
                return res;
            }
            if (request.EndYear.IsBefore(request.StartYear))
            {
                res["InvalidDate"] = "Cannot Save Company,End Of Year Date Cannot Be Lass Than Start Of Year";
                return res;
            }
            if (request.StartYear.IsAfter(GeneralFunc.GetTodayDate()))
            {
                res["InvalidDate"] = "Cannot Save Company,End Of Year Date Is In The Future";
                return res;
            }
            if(request.StartYear.AddYears(1).AddDays(-1)!=request.EndYear) {
                res["InvalidDate"] = "Cannot Run End Of Year. Start And End Year Interval Must Be One Year";
                return res;
            }
            EOYRun oEOYRun = new EOYRun();
            oEOYRun.RunStartDate = request.StartYear;
            oEOYRun.RunEndDate = request.EndYear;
            oEOYRun.RunType = "E";
            if (oEOYRun.CheckDateInEOYRun())
            {
                res["EoY"] = "Cannot Save Company. End Of Year Already Run For This Period";
                return res;
            }
            oEOYRun.RunStartDate = request.StartYear;
                return res;
        }
    }
}