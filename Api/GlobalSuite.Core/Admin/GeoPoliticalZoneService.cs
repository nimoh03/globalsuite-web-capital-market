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
        public async Task<ResponseResult> CreateGeoZones(string name)
        {
           return await Task.Run(async()=> await SaveParam(Constants.ParamTable.GEOPOLITICAL, name));
        }
        public async Task<List<Param>> GetAllGeoZones()
        {
            return await Task.Run(async () => await GetAllParams(Constants.ParamTable.GEOPOLITICAL));
        }
    }
}
