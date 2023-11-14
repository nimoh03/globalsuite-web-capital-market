using AutoMapper;
using CustomerManagement.Business;
using GL.Business;
using GlobalSuite.Core.Customers.Models;

namespace GlobalSuite.Web.Controllers.Customers.Dto
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<CustomerRequest, Customer>();
            CreateMap<Customer, PublicCustomer>().ForMember(x=>x.CustAID,
                y=>y.MapFrom(x=>x.CustAID.Trim()));
            CreateMap<Customer, CustomerResponse>();
            CreateMap<CustomerBankRequest, CustomerBank>();
            CreateMap<NextOfKinRequest, CustomerNextOfKin>();
            CreateMap<CustomerEmployerRequest, CustomerEmployer>();
            CreateMap<CustomerExtraInformationRequest, CustomerExtraInformation>();
            
            CreateMap<CustomerType, CustomerTypeResponse>();
            
            
            CreateMap<KycDocTypeCustomerTypeRequest, KYCDocType>();
            CreateMap<KYCDocType, KycDocTypeResponse>();
        }
    }
}