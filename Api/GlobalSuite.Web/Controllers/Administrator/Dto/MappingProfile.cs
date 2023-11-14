using Admin.Business;
using AutoMapper;
using GlobalSuite.Core.Admin.Models;

namespace GlobalSuite.Web.Controllers.Dto
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<UserRequest, User>(); 
            CreateMap<User, UserResponse>(); 
            CreateMap<Branch, BranchResponse>(); 
        }
    }
}