using AutoMapper;
using HR.Business;

namespace GlobalSuite.Web.Controllers.Employees.Dto
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<Employee, EmployeeResponse>();
            CreateMap<EmployeeRequest, Employee>();
        }
    }
}