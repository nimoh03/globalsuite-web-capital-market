using GlobalSuite.Core.Admin;
using GlobalSuite.Core.Customers;
using Ninject.Modules;
using GlobalSuite.Core.Accounting;
using GlobalSuite.Core.Caching;
using GlobalSuite.Core.CapitalMarket;
using GlobalSuite.Core.Hr;

namespace GlobalSuite.Core
{
    public class CoreModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IAdminService>().To<AdminService>();
            Bind<IHrService>().To<HrService>();
            Bind<ICustomerService>().To<CustomerService>();
            Bind<ITradingService>().To<TradingService>();
            Bind<IAccountingService>().To<AccountingService>();
        }
    }
}
