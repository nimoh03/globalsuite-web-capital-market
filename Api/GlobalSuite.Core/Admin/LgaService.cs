using System.Collections.Generic;
using System.Threading.Tasks;
using Admin.Business;
using BaseUtility.Business;
using GlobalSuite.Core.Helpers;

namespace GlobalSuite.Core.Admin
{
    public partial class AdminService 
    {
        public async Task<DataGeneral.SaveStatus> CreateLga(string name, int stateId)
        {
            var lga = new LGA
            {
                LGAName = name,
                State=stateId,
                SaveType=Constants.SaveType.ADDS,
                TransNo=GetTransNo(),
            };
           return await Task.Run(() =>
            {
               return lga.Save();
            });
           
        }

        private string GetTransNo()
        {
            return "001";
        }

        public async Task<List<LGA>> GetAllLgas()
        {
            var lga = new LGA();
            var lgas = await Task.Run(() => lga.GetAll().Tables[0].ToList<LGA>());
            return lgas;
        }

        public async Task<List<LGA>> GetLgaByStateId(int stateId)
        {
            var lga = new LGA
            {
                State = stateId
            };
            var lgas = await Task.Run(() => lga.GetAllByState().Tables[0].ToList<LGA>());
            return lgas;
        }
    }
}
