using System.Collections.Generic;
using System.Threading.Tasks;
using CustomerManagement.Business;
using GL.Business;
using GlobalSuite.Core.Customers.Models;
using GlobalSuite.Core.Helpers;

namespace GlobalSuite.Core.Customers
{
    public interface ICustomerService
    {
        Task<ResponseResult> CreateCustomer(Customer customer, List<CustomerBank> customerBanks, CustomerNextOfKin nextOfKin,
            CustomerEmployer employer,CustomerExtraInformation extraInformation );

        Task<List<KYCDocType>> GetKycDocTypes();
        Task<ResponseResult> SetKycDocTypeForCustomerType(long kycTransNo, int customerTypeTransNo, bool isOptional=false);
        Task<List<CustomerFieldData.KYCCompulsoryCustomer>> GetKycCompulsoryCustomer();
        Task<ResponseResult> SetCompulsoryKyc(List<(string customerFieldId, string customerFieldName, int customerFieldDataType)> compusoryKycs);
        Task<List<CustomerType>> GetAllCustomerTypes();
        Task<List<Customer>> GetCustomers(CustomerFilter filter);
    }
}
