using System.Collections.Generic;
using System.Threading.Tasks;
using BaseUtility.Business;
using GL.Business;
using GlobalSuite.Core.Admin.Models;
using GlobalSuite.Core.Helpers;

namespace GlobalSuite.Core.Admin
{
    public partial class AdminService
    {
        public async Task<List<Param>> GetAllTitles()=>
            await Task.Run(async () =>await GetAllParams(Constants.ParamTable.TITLE));
        public async Task<ResponseResult> CreateTitle(string title) => 
            await Task.Run(async () =>await SaveParam(Constants.ParamTable.TITLE, title));
    }
}
