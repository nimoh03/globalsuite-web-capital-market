using GlobalSuite.Core.Sessions;
using GlobalSuite.Infrastructure.Sessions;
using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseUtility.Business;

namespace GlobalSuite.Infrastructure
{
    public class InfrastructureModule : NinjectModule
    {
        public override void Load()
        {
            Bind<GeneralFunc>().ToSelf();
            Bind<IAppSession>().To<AppSession>().InSingletonScope();

        }
    }
}
