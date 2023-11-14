using GlobalSuite.Core.Sessions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseUtility.Business;

namespace GlobalSuite.Infrastructure.Sessions
{
    public class AppSession : GeneralFunc, IAppSession
    {
        string IAppSession.UserName {
            get
            {
                    UserName = "molo";
                return UserName;
              }
            }

        int IAppSession.CompanyNumber => 1;

        string IAppSession.UserBranchNumber => throw new NotImplementedException();

        string IAppSession.DefaultBranch => throw new NotImplementedException();

        string IAppSession.ReportName => throw new NotImplementedException();

        string IAppSession.FormModuleName => throw new NotImplementedException();
    }
}
