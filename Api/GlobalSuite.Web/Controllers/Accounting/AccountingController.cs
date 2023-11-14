using AutoMapper;
using GlobalSuite.Core.Accounting;

namespace GlobalSuite.Web.Controllers.Accounting
{
    /// <summary>
    /// Accounting Module
    /// </summary>
    public partial class AccountingController:BaseController
    {
        private readonly IAccountingService _accountingService;
        private readonly IMapper _mapper;

        public AccountingController(IAccountingService accountingService, IMapper mapper)
        {
            _accountingService = accountingService;
            _mapper = mapper;
        }
    }
}