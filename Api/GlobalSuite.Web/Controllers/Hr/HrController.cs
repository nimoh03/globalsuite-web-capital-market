using AutoMapper;
using GlobalSuite.Core.Hr;

namespace GlobalSuite.Web.Controllers.Hr
{
    public partial class HrController:BaseController
    {
        private readonly IHrService _hrService;
        private readonly IMapper _mapper;

        public HrController(IHrService hrService, IMapper mapper)
        {
            _hrService = hrService;
            _mapper = mapper;
        }
    }
}