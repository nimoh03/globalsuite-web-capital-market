using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalSuite.Application
{
    internal class BaseService
    {
        protected Task GetCurrentUser()
        {
            return Task.CompletedTask;
        }
    }
}
