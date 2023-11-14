using System.Collections.Generic;
using System.Threading.Tasks;
using Admin.Business;
using BaseUtility.Business;
using GL.Business;
using GlobalSuite.Core.Admin.Models;
using GlobalSuite.Core.Helpers;

namespace GlobalSuite.Core.Admin
{
    public partial class AdminService 
    {
        public async Task<ResponseResult> CreateOccupation(string name)
        {
           return await Task.Run(async()=> await SaveParam(Constants.ParamTable.OCCUPATIONLEVEL, name));
        }
        public async Task<List<Param>> GetAllOccupations()
        {
            return await Task.Run(async () => await GetAllParams(Constants.ParamTable.OCCUPATIONLEVEL));
        }
    }
}
