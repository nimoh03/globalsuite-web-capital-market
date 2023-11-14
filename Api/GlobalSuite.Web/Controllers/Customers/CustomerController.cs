using GlobalSuite.Core.Customers;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using AutoMapper;
using CustomerManagement.Business;
using GlobalSuite.Core.Customers.Models;
using GlobalSuite.Web.Controllers.Customers.Dto;

namespace GlobalSuite.Web.Controllers.Customers
{
    public partial class CustomerController : BaseController
    {
        private readonly ICustomerService _customerService;
        private readonly IMapper _mapper;
        public CustomerController(ICustomerService customerService, IMapper mapper)
        {
            _customerService = customerService;
            _mapper = mapper;
        }
        
        /// <summary>
        /// Get All Customers
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet, Route(GlobalSuiteRoutes.CustomerRoutes.Customers)]
        [ResponseType(typeof(List<CustomerResponse>))]
        public async Task<IHttpActionResult> GetCustomers(CustomerFilter filter)
        {
            var cus =await _customerService.GetCustomers(filter);
            var response = _mapper.Map<List<CustomerResponse>>(cus);
            return Ok(response);
        }

        /// <summary>
        /// Create a New Customer
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route(GlobalSuiteRoutes.CustomerRoutes.Customers)]
        public async Task<IHttpActionResult> Create([FromBody] CustomerRequest request)
        {
            var customer = _mapper.Map<Customer>(request);
            var nextOfKin = _mapper.Map<CustomerNextOfKin>(request.NextOfKin);
            var employer = _mapper.Map<CustomerEmployer>(request.Employer);
            var banks = _mapper.Map<List<CustomerBank>>(request.Banks);
            var extraInfo = _mapper.Map<CustomerExtraInformation>(request.ExtraInformation);
            var result = await _customerService.CreateCustomer(customer, banks, nextOfKin, employer, extraInfo);
            if (!result.IsSuccess) return BadRequest(result.ToString());
            return Ok();
        }
    }
}
