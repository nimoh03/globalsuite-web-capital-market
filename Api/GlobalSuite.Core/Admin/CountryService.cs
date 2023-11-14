using System.Collections.Generic;
using System.Threading.Tasks;
using Admin.Business;
using GlobalSuite.Core.Helpers;

namespace GlobalSuite.Core.Admin
{
    public partial class AdminService
    {
        public async Task<List<Country>> GetAllCountries()
        {
            var country = new Country();
            var countries = await Task.Run(() => country.GetAll().Tables[0].ToList<Country>());
            return countries;
        }

        public async Task<List<Country>> GetAllCountriesOrderByName()
        {
            var country = new Country();
            var countries = await Task.Run(() => country.GetAllOrderByName().Tables[0].ToList<Country>());
            return countries;
        }
    }
}
