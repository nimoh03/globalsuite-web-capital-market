using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using GlobalSuite.Web.Controllers.Customers.Dto;

namespace GlobalSuite.Web.Controllers.Customers
{
    public partial class CustomerController 
    {
        /// <summary>
        /// Get All Customer Types
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route(GlobalSuiteRoutes.CustomerRoutes.CustomerTypes)]
        public async Task<IHttpActionResult> Get()
        {
            var types = await _customerService.GetAllCustomerTypes();
            var output = _mapper.Map<List<CustomerTypeResponse>>(types);
            return Ok(output);
        }
    }
}
