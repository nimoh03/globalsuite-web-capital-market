using System.Collections.Generic;
using System.Threading.Tasks;
using CustomerManagement.Business;
using GlobalSuite.Core.Helpers;

namespace GlobalSuite.Core.Customers
{
    public partial class CustomerService 
    {
        public async Task<List<CustomerType>> GetAllCustomerTypes()
        {
            var oCustomerType = new CustomerType();
            return await Task.Run(() => oCustomerType.GetAll().Tables[0].ToList<CustomerType>());
        }
    }
}