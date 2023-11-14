using AutoMapper;
using GL.Business;
using GlobalSuite.Web.Controllers.Dto;

namespace GlobalSuite.Web
{
    public class MapperConfig
        {
            public static MapperConfiguration InitializeAutomapper()
            {
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<Company, CompanyRequest>().ReverseMap();
                      cfg.AddMaps(typeof(MapperConfig).Assembly);
                });
                 return config;
            }
        }
}
