using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using GlobalSuite.Web.Controllers.Employees.Dto;
using HR.Business;

namespace GlobalSuite.Web.Controllers.Hr
{
    public partial class HrController 
    {
        /// <summary>
        /// Get All Employees
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route(GlobalSuiteRoutes.HrRoutes.Employees)]
        public async Task< IHttpActionResult> GetEmployees()
        {
            var employees=await _hrService.GetEmployees();
            return Ok(_mapper.Map<List<EmployeeResponse>>(employees));
        }
        
        /// <summary>
        /// Create Employee
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route(GlobalSuiteRoutes.HrRoutes.Employees)]
        public async Task< IHttpActionResult> Create([FromBody] EmployeeRequest request)
        {
            var employee = _mapper.Map<Employee>(request);
            var result = await _hrService.CreateEmployee(employee);
            if (!result.IsSuccess) return BadRequest(result.ToString());
            return Ok(result);
        }
    }
}
