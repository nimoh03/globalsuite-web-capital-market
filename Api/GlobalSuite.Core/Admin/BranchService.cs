using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Admin.Business;
using GlobalSuite.Core.Admin.Models;
using GlobalSuite.Core.Helpers;

namespace GlobalSuite.Core.Admin
{
    public partial class AdminService
    {
        public async Task<List<BranchResponse>> GetBranches()
        {
            var b = new Branch();
            var ds = await Task.Run(() => b.GetAll());
            var branches = new List<BranchResponse>();
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                branches.Add(new BranchResponse
                {
                    TransNo = $"{row["Brancode"]}",
                    Name = $"{row["Branname"]}",
                    DefaultBranch = $"{row["Brancode"]}",
                    ShortCode = $"{row["ShortCode"]}",
                    Address1 = $"{row["Address1"]}",
                    Address2 = $"{row["Address2"]}",
                    Phone = $"{row["Phone"]}",
                    NameWithCode = $"{row["Brannamewithcode"]}",
                    
                });
            }
            return branches;
        }

    }
}
