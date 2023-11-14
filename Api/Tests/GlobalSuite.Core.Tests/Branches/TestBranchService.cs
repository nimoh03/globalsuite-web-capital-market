using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Admin.Business;
using GlobalSuite.Core.Admin;

namespace GlobalSuite.Core.Tests.Branches
{
    internal class TestBranchService 
    {
        public async Task<List<Branch>> GetBranches()
        {
            return await Task.FromResult(new List<Branch>
            {
                new Branch{Name="Lagos"},
                new Branch{Name="Abuja"},
            });
        }
    }
}
